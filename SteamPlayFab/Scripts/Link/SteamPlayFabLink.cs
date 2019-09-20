#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Link
{
    public sealed class SteamPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = SteamPlayFabAuth.LoginResult;
            var targetLoginResult = default(LoginResult);

            LoginWithCustomId();

            void LoginWithCustomId()
            {
                try
                {
                    PlayFabClientAPI.LoginWithCustomID(
                        new LoginWithCustomIDRequest()
                        {
                            CreateAccount = false,
                            CustomId = linkKey,
                            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                            {
                                GetUserAccountInfo = true
                            },
                            TitleId = SteamPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            SteamPlayFabAuth.LoginResult = result;

                            UnlinkCustomId();
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

            void UnlinkCustomId()
            {
                try
                {
                    PlayFabClientAPI.UnlinkCustomID(
                        new UnlinkCustomIDRequest()
                        {
                            CustomId = linkKey
                        },
                        result =>
                        {
                            CheckLinkKeyExpirationTime();
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

            void CheckLinkKeyExpirationTime()
            {
                try
                {
                    PlayFabClientAPI.GetUserData(
                        new GetUserDataRequest()
                        {
                            Keys = new List<string>()
                            {
                                LinkKeyExpirationTime
                            }
                        },
                        result =>
                        {
                            var data = result.Data;

                            if (data.TryGetValue(LinkKeyExpirationTime, out var record))
                            {
                                var now = DateTime.Now;
                                var expirationTime = DateTime.Parse(record.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                                if (now > expirationTime)
                                {
                                    resultException = new Exception($"The \"{LinkKey}\" is out of date!");

                                    Restore();
                                }
                                else
                                {
                                    CheckSteamId();
                                }
                            }
                            else
                            {
                                resultException = new Exception($"The \"{LinkKeyExpirationTime}\" is not found!");

                                Restore();
                            }
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

            void CheckSteamId()
            {
                var targetSteamId = GetSteamId(targetLoginResult);

                if (targetSteamId == null)
                {
                    LinkSteamAccount();
                }
                else
                {
                    var sourceSteamId = GetSteamId(sourceLoginResult);

                    resultException = sourceSteamId == targetSteamId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetSteamId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var steamId = accountInfo?.SteamInfo;

                return steamId?.SteamId;
            }

            void LinkSteamAccount()
            {
                try
                {
                    var authSessionTicket = SteamPlayFabAuth.CreateAuthSessionTicket();

                    PlayFabClientAPI.LinkSteamAccount(
                        new LinkSteamAccountRequest()
                        {
                            ForceLink = true,
                            SteamTicket = authSessionTicket
                        },
                        result =>
                        {
                            SteamPlayFabAuth.DestroyAuthSessionTicket(authSessionTicket);

                            onComplete();
                        },
                        error =>
                        {
                            SteamPlayFabAuth.DestroyAuthSessionTicket(authSessionTicket);
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

            void Restore()
            {
                Logout();

                void Logout()
                {
                    SteamPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    SteamPlayFabAuth.Login(ProcessException, ProcessException);
                }
            }

            void ProcessException()
            {
                ExceptionHandler.Process(resultException);

                onFailure();
            }
        }

        void ILink.RequestLinkKey(int linkKeyLenght, Action<(string LinkKey, DateTime LinkKeyExpirationTime)> onComplete, Action onFailure) => PlayFabLink.RequestLinkKey(linkKeyLenght, onComplete, onFailure);

        #endregion
        #region SteamPlayFabLink

        private const string LinkKey = nameof(LinkKey);
        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private readonly IPlayFabLink PlayFabLink;
        private readonly ISteamPlayFabAuth SteamPlayFabAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public SteamPlayFabLink(IPlayFabLink playFabLink, ISteamPlayFabAuth steamPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            SteamPlayFabAuth = steamPlayFabAuth;
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
