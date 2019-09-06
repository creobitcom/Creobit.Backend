using System;

namespace Creobit.Backend.Application
{
    public interface IApplicationData
    {
        #region IApplicationData

        void Read<T>(Action<T> onComplete, Action onFailure)
            where T : class, new();

        #endregion
    }
}
