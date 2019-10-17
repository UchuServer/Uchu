using System;
using System.Diagnostics;

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
                    Arguments = $"{location} {id} \"{MasterServer.ConfigPath}\"",
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