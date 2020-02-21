using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uchu.Api.Models;
using Uchu.Core;

namespace Uchu.Api
{
    public class ApiManager
    {
        public HttpListener Listener { get; }
        
        public string[] Prefixes { get; }
        
        public Dictionary<string, (MethodInfo, object)> Map { get; }

        public ApiManager(string[] prefixes)
        {
            Listener = new HttpListener();

            Map = new Dictionary<string, (MethodInfo, object)>();
            
            Prefixes = prefixes;
        }

        public async Task StartAsync()
        {
            foreach (var prefix in Prefixes)
            {
                Listener.Prefixes.Add(prefix);
            }
            
            Listener.Start();

            foreach (var type in typeof(ApiManager).Assembly.GetTypes())
            {
                object instance = null;
                
                foreach (var method in type.GetMethods())
                {
                    var attribute = method.GetCustomAttribute<ApiCommandAttribute>();
                    
                    if (attribute == null) continue;

                    if (instance == null)
                    {
                        instance = Activator.CreateInstance(type);
                    }

                    Map[attribute.Route] = (method, instance);
                }
            }

            while (true)
            {
                var context = await Listener.GetContextAsync();

                var request = context.Request;

                var response = context.Response;

                var parameters = (
                    from string name in request.QueryString select request.QueryString.Get(name)
                ).ToArray();
                
                string returnString;
                
                if (Map.TryGetValue(request.Url.LocalPath, out var value))
                {
                    var (info, host) = value;

                    object returnValue;
                    
                    try
                    {
                        returnValue = info.Invoke(host, parameters);
                    }
                    catch
                    {
                        returnValue = new BaseResponse
                        {
                            FailedReason = "error"
                        };
                    }
                    
                    switch (returnValue)
                    {
                        case Task<string> task:
                            returnString = await task;
                            break;
                        case string str:
                            returnString = str;
                            break;
                        case Task<object> task:
                            var obj = await task;
                            returnString = JsonConvert.SerializeObject(obj);
                            break;
                        default:
                            returnString = JsonConvert.SerializeObject(returnValue);
                            break;
                    }
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

                await response.OutputStream.WriteAsync(data);
                
                response.Close();
            }
        }
    }
}