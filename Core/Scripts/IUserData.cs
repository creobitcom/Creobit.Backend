using System;

namespace Creobit.Backend
{
    public interface IUserData
    {
        #region IUserData

        void Read<T>(Action<T> onComplete, Action onFailure)
            where T : class, new();

        void Write(object data, Action onComplete, Action onFailure);

        #endregion
    }
}