using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World.Parsers
{
    // based on https://github.com/SimonNitzsche/SimonsPublicLUStuffCode/blob/master/JS/TestsAndConcept/MissionRequirementParser/parser.js
    public static class MissionParser
    {
        private static bool Check(bool a, bool b, Mode mode)
        {
            switch (mode)
            {
                case Mode.And:
                    return a || b;

                case Mode.Or:
                    return a && b;

                case Mode.None:
                    return false;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        private static async Task<bool> IsCompletedAsync(string str, IEnumerable<Mission> completed)
        {
            if (string.IsNullOrWhiteSpace(str)) return true;
            
            if (str.Contains(':'))
            {
                var colonIndex = str.IndexOf(':');
                var misId = int.Parse(str.Substring(0, colonIndex));
                var taskIndex = int.Parse(str.Substring(colonIndex + 1));

                var chrMis = completed.FirstOrDefault(m => m.MissionId == misId);

                if (chrMis == null)
                    return false;

                using (var ctx = new CdClientContext())
                {
                    var tasks = ctx.MissionTasksTable.Where(m => m.Id == misId).ToArray();
                    
                    var task = tasks[taskIndex];
                    var chrTask = chrMis.Tasks[taskIndex];

                    return chrTask.Values.Count >= task.TargetValue;
                }
            }

            Logger.Debug($"Required mission {str}");
            var id = int.Parse(str);

            return completed.Any(c => c.MissionId == id);
        }

        public static async Task<bool> CheckPrerequiredMissionsAsync(string missions, ICollection<Mission> completed)
        {
            if (string.IsNullOrWhiteSpace(missions)) return true;
            
            var cur = new StringBuilder();
            var res = true;
            var mode = Mode.And;

            for (var i = 0; i < completed.Count; i++)
            {
                var chr = missions[i];

                switch (chr)
                {
                    case ' ':
                        break;

                    case '&':
                    case ',':
                    {
                        res = Check(res, await IsCompletedAsync(cur.ToString(), completed), mode);

                        cur.Clear();

                        if (!res)
                            return false;

                        mode = Mode.And;
                        break;
                    }

                    case '|':
                    {
                        res = Check(res, await IsCompletedAsync(cur.ToString(), completed), mode);

                        cur.Clear();

                        mode = Mode.Or;
                        break;
                    }

                    case '(':
                        res = Check(res, await CheckPrerequiredMissionsAsync(missions.Substring(i + 1), completed), mode);
                        break;

                    case ')':
                        return Check(res, await IsCompletedAsync(cur.ToString(), completed), mode);

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

            res = Check(res, await IsCompletedAsync(cur.ToString(), completed), mode);

            return res;
        }

        public static async Task<bool> AllTasksCompletedAsync(Mission mission)
        {
            using (var ctx = new CdClientContext())
            {
                var tasks = ctx.MissionTasksTable.Where(m => m.Id == mission.MissionId);

                return await tasks.AllAsync(t =>
                    mission.Tasks.Find(t2 => t2.TaskId == t.Uid).Values.Count >= t.TargetValue);
            }
        }

        public static object[] GetTargets(MissionTasks missionTask)
        {
            var targets = new List<object>();

            if (missionTask.Target != null)
            {
                targets.Add(missionTask.Target);
            }

            if (missionTask.TargetGroup != null)
            {
                targets.AddRange(missionTask.TargetGroup.Trim().Split(',').Where(c => !string.IsNullOrEmpty(c))
                    .Select(c => int.TryParse(c.Trim(), out var num) ? (object) num : c.Trim()));
            }

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