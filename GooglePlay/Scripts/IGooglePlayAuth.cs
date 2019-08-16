#if CREOBIT_BACKEND_GOOGLEPLAY && UNITY_ANDROID
using System;

namespace Creobit.Backend
{
    public interface IGooglePlayAuth : IAuth
    {
        #region IGooglePlayAuth

        void GetServerAuthCode(Action<string> onComplete, Action onFailure);

        #endregion
    }
}
#endif
