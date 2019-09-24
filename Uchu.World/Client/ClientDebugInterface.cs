using System.Diagnostics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Client
{
    public class ClientDebugInterface
    {
        //private readonly string _path;
        //private readonly WorldServer _worldServer;

        public readonly Process ClientProcess;

        public ClientDebugInterface(Process process, Zone zone)
        {
            ClientProcess = process;

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(50);
                    if (ClientProcess.StandardOutput.EndOfStream) continue;

                    await ProcessOutputAsync(ClientProcess.StandardOutput.ReadLine(), false);
                }
            });

            /*ClientProcess.OutputDataReceived += async (sender, args) => await ProcessOutput(args.Data, false);
            ClientProcess.ErrorDataReceived += async (sender, args) => await ProcessOutput(args.Data, true);

            ClientProcess.BeginOutputReadLine();
            ClientProcess.BeginErrorReadLine();*/
        }

        private Task ProcessOutputAsync(string line, bool error)
        {
            Logger.Log($"CLIENT: {line}", error ? LogLevel.Error : LogLevel.Debug);

            return Task.CompletedTask;
        }
    }
}