using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#if DEBUG
using System.Threading;
#endif
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
        
        public List<ZoneId> Zones { get; set; } = new List<ZoneId>();
        
        public bool Ready { get; set; }

        public ServerInstance(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Starts the server instance.
        /// </summary>
        /// <param name="location">Executable location to use.</param>
        /// <param name="dotnet">Location of the dotnet command.</param>
        public void Start(string location, string dotnet)
        {
#if DEBUG
            if (MasterServer.Config.DebugConfig.StartInstancesAsThreads)
                this.StartInstanceAsThread();
            else
#endif
                this.StartInstanceAsProcess(location, dotnet);
        }

#if DEBUG
        public void StartInstanceAsThread()
        {
            new Thread(() =>
            {
                var program = new Uchu.Instance.Program();
                program.Start(new[] { Id.ToString(), MasterServer.ConfigPath });
            }).Start();
        }
#endif

        public void StartInstanceAsProcess(string location, string dotnet)
            {
            // Determine if the dotnet command should be used.
            // If the default (dotnet) is used, the system PATH is checked.
            var useDotNet = !string.IsNullOrWhiteSpace(dotnet);
            if (useDotNet && dotnet?.ToLower() == "dotnet" && !File.Exists(dotnet))
            {
                var pathDirectories = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(new[] { ';', ':' });
                var dotNetInPath = pathDirectories.Any(pathDirectory => File.Exists(Path.Combine(pathDirectory, dotnet)));
                if (!dotNetInPath)
                {
                    useDotNet = false;
                }
            }

            // Adjust the file name.
            // If dotnet isn't used, the file name may need to be corrected to have .exe or no extension.
            var file = useDotNet ? dotnet : location;
            if (!useDotNet && file.ToLower().EndsWith(".dll"))
            {
                var parentDirectory = Path.GetDirectoryName(file) ?? "";
                var baseFileName = Path.GetFileNameWithoutExtension(file);
                var baseFileLocation = Path.Combine(parentDirectory, baseFileName);
                if (File.Exists(baseFileLocation + ".exe"))
                {
                    file = baseFileLocation + ".exe";
                } else if (File.Exists(baseFileLocation))
                {
                    file = baseFileLocation;
                }
            }
            
            // Create and start the process.
            this.Process = new Process
            {
                StartInfo =
                {
                    FileName = file,
                    WorkingDirectory = useDotNet ? Path.GetDirectoryName(location) : Directory.GetCurrentDirectory(),
                    Arguments = (useDotNet ? $"{Path.GetFileName(location)} " : "") + $"{Id} \"{MasterServer.ConfigPath}\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                }
            };
            this.Process.Start();
        }
    }
}
