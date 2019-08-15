using PlayFab;

namespace Creobit.Backend
{
    public interface IPlayFabErrorHandler
    {
        #region IPlayFabErrorHandler

        void Process(PlayFabError playFabError);

        #endregion
    }
}