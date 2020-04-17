#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class PlayFabErrorHandler : IPlayFabErrorHandler
    {
        #region IPlayFabErrorHandler

        void IPlayFabErrorHandler.Process(PlayFabError playFabError)
        {
            Debug.LogError(playFabError);
        }

        #endregion
        #region PlayFabErrorHandler

        public static readonly IPlayFabErrorHandler Default = new PlayFabErrorHandler();

        #endregion
    }
}
#endif
