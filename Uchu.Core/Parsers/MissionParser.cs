using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uchu.Core
{
    // based on https://github.com/SimonNitzsche/SimonsPublicLUStuffCode/blob/master/JS/TestsAndConcept/MissionRequirementParser/parser.js
    public class MissionParser
    {
        private readonly CDClient _cdClient;

        public MissionParser(CDClient cdClient)
        {
            _cdClient = cdClient;
        }

        private bool _check(bool a, bool b, Mode mode)
        {
            switch (mode)
            {
                case Mode.And:
                    return a || b;

                case Mode.Or:
                    return a && b;

                default:
                    return false;
            }
        }

        private async Task<bool> _isCompletedAsync(string str, ICollection<Mission> completed)
        {
            if (str.Contains(':'))
            {
                var colonIndex = str.IndexOf(':');
                var misId = int.Parse(str.Substring(0, colonIndex));
                var taskIndex = int.Parse(str.Substring(colonIndex + 1));

                var chrMis = completed.FirstOrDefault(m => m.MissionId == misId);

                if (chrMis == null)
                    return false;

                var tasks = await _cdClient.GetMissionTasksAsync(misId);
                var task = tasks[taskIndex];
                var chrTask = chrMis.Tasks[taskIndex];

                return chrTask.Values.Count >= task.TargetValue;
            }

            var id = int.Parse(str);

            return completed.Any(c => c.MissionId == id);
        }

        public async Task<bool> CheckPrerequiredMissionsAsync(string missions, ICollection<Mission> completed)
        {
            var cur = new StringBuilder();
            var res = true;
            var mode = Mode.None;

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
                        res = _check(res, await _isCompletedAsync(cur.ToString(), completed), mode);

                        cur.Clear();

                        if (!res)
                            return false;

                        mode = Mode.And;
                        break;
                    }

                    case '|':
                    {
                        res = _check(res, await _isCompletedAsync(cur.ToString(), completed), mode);

                        cur.Clear();

                        mode = Mode.Or;
                        break;
                    }

                    case '(':
                        res = _check(res, await CheckPrerequiredMissionsAsync(missions.Substring(i + 1), completed), mode);
                        break;

                    case ')':
                        return _check(res, await _isCompletedAsync(cur.ToString(), completed), mode);

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

            res = _check(res, await _isCompletedAsync(cur.ToString(), completed), mode);

            return res;
        }

        private enum Mode
        {
            None,
            And,
            Or
        }
    }
}