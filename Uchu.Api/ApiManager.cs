using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uchu.Api.Models;

namespace Uchu.Api
{
    public class ApiManager
    {
        public HttpListener Listener { get; }
        
        public HttpClient Client { get; }
        
        public string Protocol { get; }
        
        public string Domain { get; }
        
        public int Port { get; private set; }
        
        public Dictionary<string, (MethodInfo, object)> Map { get; }

        public event Func<Task> OnLoaded;

        private bool _running;

        public ApiManager(string protocol, string domain)
        {
            Listener = new HttpListener();

            Client = new HttpClient();

            Map = new Dictionary<string, (MethodInfo, object)>();

            Protocol = protocol;

            Domain = domain;
        }

        public async Task StartAsync(int port)
        {
            Port = port;
            
            var prefix = $"{Protocol}://{Domain}:{Port}/";
            
            Listener.Prefixes.Add(prefix);
            
            Listener.Start();

            _running = true;

            var first = true;
            
            while (_running)
            {
                if (OnLoaded != default && first)
                {
                    first = false;

                    var __ = Task.Run(async () =>
                    {
                        await OnLoaded.Invoke();
                    });
                }

                var context = await Listener.GetContextAsync();

                var _ = Task.Run(async () =>
                {
                    var request = context.Request;
                    
                    var response = context.Response;

                    var parameters = (
                        from string name in request.QueryString select request.QueryString.Get(name)
                    ).ToArray();

                    string returnString;

                    var query = request.Url.LocalPath.Remove(0, 1);

                    if (Map.TryGetValue(query, out var value))
                    {
                        var (info, host) = value;

                        object returnValue;

                        try
                        {
                            returnValue = info.Invoke(host, parameters);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            
                            returnValue = new BaseResponse
                            {
                                FailedReason = "error"
                            };
                        }

                        if (returnValue is Task<object> task)
                        {
                            returnValue = await task;
                        }

                        returnString = returnValue switch
                        {
                            string str => str,
                            _ => JsonConvert.SerializeObject(returnValue)
                        };
                    }
                    else
                    {
                        returnString = JsonConvert.SerializeObject(new BaseResponse
                        {
                            FailedReason = "invalid"
                        });
                    }

                    response.ContentType = "application/json";
                    response.ContentEncoding = Encoding.UTF8;

                    var data = Encoding.UTF8.GetBytes(returnString);

                    response.ContentLength64 = data.LongLength;

                    var output = response.OutputStream;

                    await output.WriteAsync(data);

                    response.Close();
                });
            }
        }

        public void RegisterCommandCollection<T>(params object[] parameters)
        {
            var type = typeof(T);

            object instance = null;
            
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<ApiCommandAttribute>();
                
                if (attribute == null) continue;

                if (instance == null)
                {
                    instance = Activator.CreateInstance(type, parameters);
                }

                Map[attribute.Route] = (method, instance);
            }
        }
        
        public void Close()
        {
            _running = false;
            
            Listener.Close();
        }

        public async Task<T> RunCommandAsync<T>(int port, string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            var prefix = $"{Protocol}://{Domain}:{port}/";

            var url = $"{prefix}{command}";

            string json;
            
            try
            {
                json = await Client.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                return default;
            }

            var response = JsonConvert.DeserializeObject<T>(json);

            return response;
        }
    }
}