using System;
using System.Collections.Generic;
using Uchu.Core;

namespace Uchu.World
{
    internal static class Program
    {
        private enum ArgumentState
        {
            Port,
            Zone
        }
        
        private static void Main(string[] args)
        {
            var port = 2003;
            List<ZoneId> zoneIds = default;
            var preload = false;
            
            var state = ArgumentState.Port;
            
            foreach (var arg in args)
            {
                var normalizedArg = arg.ToLower();

                switch (normalizedArg)
                {
                    case "-p":
                    case "-port":
                        state = ArgumentState.Port;
                        break;
                    case "-z":
                    case "-zones":
                        if (zoneIds == default) zoneIds = new List<ZoneId>();
                        
                        state = ArgumentState.Zone;
                        break;
                    case "-pl":
                    case "-preload":
                        preload = true;
                        break;
                    default:
                        switch (state)
                        {
                            case ArgumentState.Port:
                                if (!uint.TryParse(arg, out var argumentPort))
                                    throw new ArgumentException($"{arg} is not a valid port.");
                                
                                port = (int) argumentPort;
                                break;
                            case ArgumentState.Zone:
                                if (!Enum.TryParse<ZoneId>(arg, out var zone))
                                    throw new ArgumentException($"{arg} is not a valid zone.");
                                
                                zoneIds?.Add(zone);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        break;
                }
            }
            
            var server = new WorldServer(port, zoneIds?.ToArray(), preload);

            server.Start();
        }
    }
}