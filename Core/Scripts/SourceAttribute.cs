using System;

namespace Creobit.Backend
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SourceAttribute : Attribute
    {
        #region SourceAttribute

        public bool Public
        {
            get;
            set;
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        #endregion
    }
}
