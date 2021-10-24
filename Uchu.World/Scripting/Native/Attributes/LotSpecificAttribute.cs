using System;

namespace Uchu.World.Scripting.Native
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LotSpecific : Attribute
    {
        /// <summary>
        /// LOT of the script that the class applies to.
        /// </summary>
        public Lot Lot { get; }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="lot">LOT of the script that the class applies to.</param>
        public LotSpecific(int lot)
        {
            this.Lot = lot;
        }
    }
}