using System;
using System.Diagnostics;
using System.IO;

namespace Uchu.Master
{
    public class ManagedServer
    {
        public readonly Process Process;

        public readonly Guid Id;

        public ManagedServer(Guid id, string location, string dotnet)
        {
            var useDotNet = !string.IsNullOrWhiteSpace(dotnet);

            var file = useDotNet ? dotnet : location;
            
            Id = id;
            Process = new Process
            {
                StartInfo =
                {
                    FileName = file,
                    WorkingDirectory = useDotNet ? Path.GetDirectoryName(location) : Directory.GetCurrentDirectory(),
                    Arguments = (useDotNet ? $"{Path.GetFileName(location)} " : "") + $"{id} \"{MasterServer.ConfigPath}\"",
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