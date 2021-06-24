using System;

namespace Uchu.World.Scripting.Native
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectSpecific : Attribute
    {
        /// <summary>
        /// Name of the script that the class applies to.
        /// </summary>
        public string ScriptName { get; }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="scriptName">Name of the script that the class applies to.</param>
        public ObjectSpecific(string scriptName)
        {
            this.ScriptName = scriptName;
        }
    }
}