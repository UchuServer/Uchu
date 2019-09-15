using System;

namespace Uchu.World
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerComponentAttribute : Attribute
    {
        public ReplicaComponentsId Id { get; set; }
    }
}