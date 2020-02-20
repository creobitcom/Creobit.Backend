#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Creobit.Backend.Link
{
    public sealed class PlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILinkCode.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

            onFailure();
        }

        void ILinkCode.RequestLinkKey(int linkKeyLenght, Action<(string LinkKey, DateTime LinkKeyExpirationTime)> onComplete, Action onFailure)
        {
            var linkKey = CreateLinkKey(linkKeyLenght);

            LinkCustomId();

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

            void LinkCustomId()
            {
                try
                {
                    PlayFabClientAPI.LinkCustomID(
                        new LinkCustomIDRequest()
                        {
                            CustomId = linkKey
                        },
                        result =>
                        {
                            WriteLinkKeyExpirationTime();
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
            }

            void WriteLinkKeyExpirationTime()
            {
                try
                {
                    var now = DateTime.Now;
                    var expirationTime = now + TimeSpan.FromSeconds(AvailabilityTime);

                    PlayFabClientAPI.UpdateUserData(
                        new UpdateUserDataRequest()
                        {
                            Data = new Dictionary<string, string>()
                            {
                                { LinkKeyExpirationTime, expirationTime.ToString("O", CultureInfo.InvariantCulture) }
                            },
                            Permission = UserDataPermission.Private
                        },
                        result =>
                        {
                            onComplete((linkKey, expirationTime));
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
            }
        }

        #endregion
        #region PlayFabLink

        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private float? _availabilityTime;
        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public float AvailabilityTime
        {
            get => _availabilityTime ?? 60f;
            set => _availabilityTime = value;
        }

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
