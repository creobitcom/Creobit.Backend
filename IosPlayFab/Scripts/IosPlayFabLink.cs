#if CREOBIT_BACKEND_IOS && CREOBIT_BACKEND_PLAYFAB && UNITY_IOS
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class IosPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = IosPlayFabAuth.LoginResult;
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
                            TitleId = IosPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            IosPlayFabAuth.LoginResult = result;

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
                            CheckIosDeviceId();
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

            void CheckIosDeviceId()
            {
                var targetIosDeviceId = GetIosDeviceId(targetLoginResult);

                if (targetIosDeviceId == null || targetIosDeviceId == linkKey)
                {
                    LinkIosDeviceId();
                }
                else
                {
                    var sourceIosDeviceId = GetIosDeviceId(sourceLoginResult);

                    resultException = sourceIosDeviceId == targetIosDeviceId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetIosDeviceId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var iosDeviceInfo = accountInfo?.IosDeviceInfo;

                return iosDeviceInfo?.IosDeviceId;
            }

            void LinkIosDeviceId()
            {
                try
                {
                    PlayFabClientAPI.LinkIOSDeviceID(
                        new LinkIOSDeviceIDRequest()
                        {
                            DeviceId = SystemInfo.deviceUniqueIdentifier,
                            DeviceModel = SystemInfo.deviceModel,
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
                    IosPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    IosPlayFabAuth.Login(ProcessException, ProcessException);
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
        private readonly IIosPlayFabAuth IosPlayFabAuth;

        public IosPlayFabLink(IPlayFabLink playFabLink, IIosPlayFabAuth iosPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            IosPlayFabAuth = iosPlayFabAuth;
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
