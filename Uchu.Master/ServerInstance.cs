using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Uchu.Core;

namespace Uchu.Master
{
    public class ServerInstance
    {
        public Process Process { get; private set; }

        public Guid Id { get; }
        
        public ServerType ServerType { get; set; }
        
        public int ServerPort { get; set; }
        
        public int ApiPort { get; set; }
        
        public List<ZoneId> Zones { get; set; }
        
        public bool Ready { get; set; }

        public ServerInstance(Guid id)
        {
            Id = id;
            
            Zones = new List<ZoneId>();
        }

        public void Start(string location, string dotnet)
        {
            var useDotNet = !string.IsNullOrWhiteSpace(dotnet);

            var file = useDotNet ? dotnet : location;
            
            Process = new Process
            {
                StartInfo =
                {
                    FileName = file,
                    WorkingDirectory = useDotNet ? Path.GetDirectoryName(location) : Directory.GetCurrentDirectory(),
                    Arguments = (useDotNet ? $"{Path.GetFileName(location)} " : "") + $"{Id} \"{MasterServer.ConfigPath}\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };
            
            Process.Start();
        }
    }
}