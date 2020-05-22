using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Client
{
    // based on https://github.com/SimonNitzsche/SimonsPublicLUStuffCode/blob/master/JS/TestsAndConcept/MissionRequirementParser/parser.js
    public static class MissionParser
    {
        private static bool Check(bool a, bool b, Mode mode)
        {
            return mode switch
            {
                Mode.And => (a && b),
                Mode.Or => (a || b),
                Mode.None => false,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private static bool IsCompleted(string str, Mission[] completed)
        {
            if (string.IsNullOrWhiteSpace(str)) return true;

            if (str.Contains(':'))
            {
                var colonIndex = str.IndexOf(':');
                var misId = int.Parse(str.Substring(0, colonIndex));
                var taskIndex = int.Parse(str.Substring(colonIndex + 1));

                var chrMis = completed.FirstOrDefault(m => m.MissionId == misId);

                if (chrMis == default)
                    return false;

                using var cdClient = new CdClientContext();
                var tasks = cdClient.MissionTasksTable.Where(m => m.Id == misId).ToArray();

                var task = tasks[taskIndex];
                var chrTask = chrMis.Tasks[taskIndex];

                return chrTask.Values.Count >= task.TargetValue;
            }

            var id = int.Parse(str);

            return completed.Any(c => c.MissionId == id);
        }

        public static bool CheckPrerequiredMissions(string missions, Mission[] completed)
        {
            if (string.IsNullOrWhiteSpace(missions)) return true;

            Logger.Information($"Pre: {missions}");

            missions = missions.Replace(" ", "");

            var cur = new StringBuilder();
            var res = true;
            var mode = Mode.And;

            for (var i = 0; i < missions.Length; i++)
            {
                var chr = missions[i];

                switch (chr)
                {
                    case ' ':
                        break;

                    case '&':
                    case ',':
                    {
                        res = Check(res, IsCompleted(cur.ToString(), completed), mode);

                        cur.Clear();

                        if (!res)
                            return false;

                        mode = Mode.And;
                        break;
                    }

                    case '|':
                    {
                        res = Check(res, IsCompleted(cur.ToString(), completed), mode);

                        cur.Clear();

                        mode = Mode.Or;
                        break;
                    }

                    case '(':
                        res = Check(res, CheckPrerequiredMissions(missions.Substring(i + 1), completed), mode);
                        break;

                    case ')':
                        return Check(res, IsCompleted(cur.ToString(), completed), mode);

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case ':':
                        cur.Append(chr);
                        break;
                }
            }

            res = Check(res, IsCompleted(cur.ToString(), completed), mode);

            return res;
        }

        public static Lot[] GetTargets(MissionTasks missionTask)
        {
            var targets = new List<Lot>();

            if (missionTask.Target != null) targets.Add(missionTask.Target ?? 0);

            var rawTargets = new List<string>();

            if (missionTask.TargetGroup != default)
            {
                var paramTargets = missionTask.TargetGroup
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();

                rawTargets.AddRange(paramTargets);
            }

            if (missionTask.TaskParam1 != default)
            {
                var paramTargets = missionTask.TaskParam1
                    .Replace(" ", "")
                    .Split(',')
                    .Where(c => !string.IsNullOrEmpty(c)).ToList();

                rawTargets.AddRange(paramTargets);
            }

            foreach (var rawTarget in rawTargets)
                if (int.TryParse(rawTarget, out var target))
                    targets.Add(target);

            return targets.ToArray();
        }

        private enum Mode
        {
            None,
            And,
            Or
        }
    }
}