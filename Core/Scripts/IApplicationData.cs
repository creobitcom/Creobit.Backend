using System;

namespace Creobit.Backend
{
    public interface IApplicationData
    {
        #region IApplicationData

        void Read<T>(Action<T> onComplete, Action onFailure)
            where T : class, new();

        #endregion
    }
}