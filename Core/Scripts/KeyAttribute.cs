using System;

namespace Creobit.Backend
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KeyAttribute : Attribute
    {
        #region KeyAttribute

        public readonly object Key;

        public KeyAttribute(object key)
        {
            Key = key;
        }

        #endregion
    }
}