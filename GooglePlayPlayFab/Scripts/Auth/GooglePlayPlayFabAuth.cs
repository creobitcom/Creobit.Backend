#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
{
    public sealed class GooglePlayPlayFabAuth : PlayFabAuthDecorator
    {
        #region GooglePlayPlayFabAuth

        private readonly IGooglePlayAuth GooglePlayAuth;

        public GooglePlayPlayFabAuth(IPlayFabAuth playFabAuth, IGooglePlayAuth googlePlayAuth)
            : base(playFabAuth)
        {
            GooglePlayAuth = googlePlayAuth;
        }

        #endregion
        #region PlayFabAuthDecorator
        protected override void Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            GooglePlayAuth.Login(PlayFabLogin, onFailure);

            void PlayFabLogin()
            {
                GooglePlayAuth.GetServerAuthCode(
                    serverAuthCode =>
                    {
                        try
                        {
                            PlayFabClientAPI.LoginWithGoogleAccount
                            (
                                new LoginWithGoogleAccountRequest()
                                {
                                    CreateAccount = doCreateAccount,
                                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                                    {
                                        GetUserAccountInfo = true
                                    },
                                    ServerAuthCode = serverAuthCode,
                                    TitleId = TitleId
                                },
                                result =>
                                {
                                    LoginResult = result;

                                    onComplete();
                                },
                                error =>
                                {
                                    onFailure();
                                }
                            );
                        }
                        catch (Exception)
                        {
                            onFailure();
                        }
                    }, onFailure);
            }
        }

        #endregion
    }
}
#endif
