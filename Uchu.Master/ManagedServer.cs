using System;
using System.Diagnostics;
using System.IO;

namespace Uchu.Master
{
    public class ManagedServer
    {
        public readonly Process Process;

        public readonly Guid Id;

        public ManagedServer(Guid id, string location)
        {
            Id = id;
            Process = new Process
            {
                StartInfo =
                {
                    FileName = "dotnet",
                    WorkingDirectory = Path.GetDirectoryName(location),
                    Arguments = $"{Path.GetFileName(location)} {id} \"{MasterServer.ConfigPath}\"",
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