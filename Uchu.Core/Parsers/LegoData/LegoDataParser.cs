using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Uchu.Core
{
    public static class LegoDataParser
    {
        public static Dictionary<string, object> ParseText(string text)
        {
            var dict = new Dictionary<string, object>();
            var lines = text.Replace("\r", "").Split('\n');

            foreach (var line in lines)
            {
                var firstEqual = line.IndexOf('=');
                var firstColon = line.IndexOf(':');
                var key = line.Substring(0, firstEqual);
                var type = int.Parse(line.Substring(firstEqual + 1, firstColon - firstEqual - 1));
                var val = line.Substring(firstColon + 1);

                switch (type)
                {
                    case 1:
                    case 2:
                        dict[key] = int.Parse(val);
                        break;

                    case 3:
                        dict[key] = float.Parse(val);
                        break;

                    case 4:
                        dict[key] = double.Parse(val);
                        break;

                    case 5:
                    case 6:
                        dict[key] = uint.Parse(val);
                        break;

                    case 7:
                        dict[key] = int.Parse(val) == 1;
                        break;

                    case 8:
                    case 9:
                        dict[key] = long.Parse(val);
                        break;

                    default:
                        if (val.Contains('\u001F'))
                        {
                            var floats = val.Split('\u001F').Select(float.Parse).ToArray();

                            dict[key] =
                                floats.Length == 1 ? floats[0] :
                                floats.Length == 2 ? new Vector2(floats[0], floats[1]) :
                                floats.Length == 3 ? new Vector3(floats[0], floats[1], floats[2]) :
                                floats.Length == 4 ? new Vector4(floats[0], floats[1], floats[2], floats[3]) :
                                (object) val;
                        }
                        else
                            dict[key] = val;
                        break;
                }
            }

            return dict;
        }
    }
}