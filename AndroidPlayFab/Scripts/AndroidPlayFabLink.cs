#if CREOBIT_BACKEND_ANDROIDPLAYFAB && CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class AndroidPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = AndroidPlayFabAuth.LoginResult;
            var targetLoginResult = default(LoginResult);

            LoginWithCustomID();

            void LoginWithCustomID()
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
                            TitleId = AndroidPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            AndroidPlayFabAuth.LoginResult = result;

                            UnlinkCustomID();
                        },
                        error =>
                        {
                            PlayFabErrorHandler?.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }

            void UnlinkCustomID()
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
                            CheckAndroidDeviceId();
                        },
                        error =>
                        {
                            PlayFabErrorHandler?.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }

            void CheckAndroidDeviceId()
            {
                var targetAndroidDeviceId = GetAndroidDeviceID(targetLoginResult);

                if (targetAndroidDeviceId == null || targetAndroidDeviceId == linkKey)
                {
                    LinkAndroidDeviceID();
                }
                else
                {
                    var sourceAndroidDeviceId = GetAndroidDeviceID(sourceLoginResult);

                    resultException = sourceAndroidDeviceId == targetAndroidDeviceId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetAndroidDeviceID(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var androidDeviceInfo = accountInfo?.AndroidDeviceInfo;

                return androidDeviceInfo?.AndroidDeviceId;
            }

            void LinkAndroidDeviceID()
            {
                try
                {
                    PlayFabClientAPI.LinkAndroidDeviceID(
                        new LinkAndroidDeviceIDRequest()
                        {
                            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                            AndroidDevice = SystemInfo.deviceModel,
                            ForceLink = true,
                            OS = SystemInfo.operatingSystem
                        },
                        result =>
                        {
                            onComplete();
                        },
                        error =>
                        {
                            PlayFabErrorHandler?.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }

            void Restore()
            {
                Logout();

                void Logout()
                {
                    AndroidPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    AndroidPlayFabAuth.Login(ProcessException, ProcessException);
                }
            }

            void ProcessException()
            {
                ExceptionHandler?.Process(resultException);

                onFailure();
            }
        }

        void ILink.RequestLinkKey(int linkKeyLenght, Action<string> onComplete, Action onFailure) => PlayFabLink.RequestLinkKey(linkKeyLenght, onComplete, onFailure);

        #endregion
        #region IosPlayFabLink

        private readonly IPlayFabLink PlayFabLink;
        private readonly IAndroidPlayFabAuth AndroidPlayFabAuth;

        public AndroidPlayFabLink(IPlayFabLink playFabLink, IAndroidPlayFabAuth androidPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            AndroidPlayFabAuth = androidPlayFabAuth;
        }

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get;
            set;
        } = Backend.PlayFabErrorHandler.Default;

        #endregion
    }
}
#endif
