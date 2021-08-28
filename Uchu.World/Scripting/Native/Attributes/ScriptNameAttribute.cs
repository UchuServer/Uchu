using System;

namespace Uchu.World.Scripting.Native
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScriptName : Attribute
    {
        /// <summary>
        /// Name of the script that the class applies to.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="scriptName">Name of the script that the class applies to.</param>
        public ScriptName(string scriptName)
        {
            this.Name = scriptName;
        }
    }
}
