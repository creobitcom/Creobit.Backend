#if UNITY_ANDROID && !UNITY_EDITOR
#define ENABLED
#endif
#if ENABLED
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public sealed class GooglePlayAuth : IGooglePlayAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayGamesPlatform.Instance.IsAuthenticated();

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                onComplete();

                return;
            }

            Execute();

            async void Execute()
            {
                var wait = true;
                var successArg = default(bool);
                var msgArg = default(string);

                PlayGamesPlatform.Instance.Authenticate(
                    (success, msg) =>
                    {
                        wait = false;
                        successArg = success;
                        msgArg = msg;
                    });

                while (wait)
                {
                    await Task.Delay(MillisecondsDelay);
                }

                if (successArg)
                {
                    onComplete();
                }
                else
                {
                    var exception = new Exception(msgArg);

                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            try
            {
                PlayGamesPlatform.Instance.SignOut();

                onComplete();
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region IGooglePlayAuth

        void IGooglePlayAuth.GetServerAuthCode(Action<string> onComplete, Action onFailure)
        {
            Execute();

            async void Execute()
            {
                var wait = true;
                var serverAuthCodeArg = default(string);

                PlayGamesPlatform.Instance.GetAnotherServerAuthCode(true,
                    serverAuthCode =>
                    {
                        wait = false;
                        serverAuthCodeArg = serverAuthCode;
                    });

                while (wait)
                {
                    await Task.Delay(MillisecondsDelay);
                }

                if (string.IsNullOrWhiteSpace(serverAuthCodeArg))
                {
                    var exception = new Exception("GetAnotherServerAuthCode error!");

                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
                else
                {
                    onComplete(serverAuthCodeArg);
                }
            }
        }

        #endregion
        #region GooglePlayAuth

        private const int MillisecondsDelay = 10;

        public GooglePlayAuth()
        {
            var configuration = new PlayGamesClientConfiguration.Builder()
                .AddOauthScope("profile")
                .RequestServerAuthCode(false)
                .Build();

            PlayGamesPlatform.InitializeInstance(configuration);
        }

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        #endregion
    }
}
#else
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayAuth : IGooglePlayAuth
    {
        bool IAuth.IsLoggedIn => throw new NotSupportedException();

        void IAuth.Login(Action onComplete, Action onFailure) => throw new NotSupportedException();

        void IAuth.Logout(Action onComplete, Action onFailure) => throw new NotSupportedException();

        void IGooglePlayAuth.GetServerAuthCode(Action<string> onComplete, Action onFailure) => throw new NotSupportedException();

        public GooglePlayAuth()
        {
        }
    }
}
#endif