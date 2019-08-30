using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Internal;

namespace Uchu.World
{
    internal static class Program
    {
        internal class LuaEntry
        {
            internal string Path;
            internal int Count;
        }
        
        private static void Main(string[] args)
        {
            /*
            var list = new List<LuaEntry>();

            var baseLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            foreach (var luaFile in Directory.GetFiles($"{baseLocation}/scripts/", "*.lua", SearchOption.AllDirectories))
            {
                var lines = File.ReadAllLines(luaFile);

                for (var index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    
                    if (!line.StartsWith("require(")) continue;
                    
                    if (list.All(l => l.Path != line))
                    {
                        var final = line.Replace('\"', '\'').Replace("require('", "").Replace("')", "");
                        var parts = final.Split('/');
                        final = parts.Last();

                        var entry = new LuaEntry
                        {
                            Path = $"require('{final}')",
                            Count = 1
                        };
                        
                        list.Add(entry);

                        lines[index] = entry.Path;
                    }
                    else
                    {
                        var entry = list.First(l => l.Path == line);
                        var valueTuple = entry;
                        valueTuple.Count++;

                        lines[index] = entry.Path;
                    }

                    File.WriteAllLines($"{baseLocation}/lua_scripts/{Path.GetFileName(luaFile)}", lines);
                }
            }

            foreach (var req in list)
            {
                Console.WriteLine($"{req.Path} x {req.Count}");
            }
            */
            
            var server = new WorldServer(2003);

            server.Start();
        }
    }
}