using System;

namespace Uchu.Core.Scriptable
{
    /// <inheritdoc />
    /// <summary>
    ///     Auto assign a script to objects holding a Component, a LOT, a Name, of any combination of the
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoAssignAttribute : Attribute
    {
        public readonly Type Component;
        public readonly int LOT;
        public readonly string Name;

        /// <inheritdoc />
        /// <summary>
        ///     Auto assign this script to objects on world load.
        ///     All of these requirements have to be meet by an object for this script to be assigned. Consider adding
        ///     multiple AutoAssignAttributes to cover a wider range or objects.
        /// </summary>
        /// <param name="component">Auto assign this script to every object holding this component type</param>
        /// <param name="name">Auto assign this script to every object holding this name</param>
        /// <param name="lot">Auto assign this script to every object holding this LOT</param>
        /// <exception cref="T:System.Exception"></exception>
        public AutoAssignAttribute(Type component = null, string name = null, int lot = 0)
        {
            if (component != null && component.BaseType != typeof(ReplicaComponent))
                throw new ArgumentException(
                    $"Auto Assign to Component {component} is not of type {typeof(IReplicaComponent)}!");

            Component = component;
            Name = name;
            LOT = lot;
        }
    }
}