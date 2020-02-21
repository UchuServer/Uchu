using System;

namespace Uchu.Api
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiCommandAttribute : Attribute
    {
        public string Route { get; }

        public ApiCommandAttribute(string route)
        {
            Route = route;
        }
    }
}