#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Text;

namespace Creobit.Backend.Link
{
    public sealed class PlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

            onFailure();
        }

        void ILink.RequestLinkKey(int linkKeyLenght, Action<string> onComplete, Action onFailure)
        {
            try
            {
                var linkKey = CreateLinkKey(linkKeyLenght);

                PlayFabClientAPI.LinkCustomID(
                    new LinkCustomIDRequest()
                    {
                        CustomId = linkKey
                    },
                    result =>
                    {
                        onComplete(linkKey);
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }

            string CreateLinkKey(int lenght)
            {
                var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var random = new Random();
                var stringBuilder = new StringBuilder(6);

                for (var i = 0; i < lenght; ++i)
                {
                    var character = characters[random.Next(0, characters.Length)];

                    stringBuilder.Append(character);
                }

                return stringBuilder.ToString();
            }
        }

        #endregion
        #region PlayFabLink

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        #endregion
    }
}
#endif
