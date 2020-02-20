#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend.Auth
{
    public sealed class OpenIdPlayFabAuth : PlayFabAuthDecorator
    {
        #region PlayFabAuthDecorator

        protected override void Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            OpenIdProvider.RequestToken(token => Login(token, doCreateAccount, onComplete, onFailure));
        }

        #endregion
        #region OpenIdPlayFabAuth

        private readonly IOpenIdProvider OpenIdProvider;

        public OpenIdPlayFabAuth(IPlayFabAuth playFabAuth, IOpenIdProvider openIdProvider)
            :base(playFabAuth)
        {
            OpenIdProvider = openIdProvider;
        }

        private void Login(string idToken, bool doCreateAccount, Action onComplete, Action onFailure)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                onFailure?.Invoke();
                return;
            }

            var request = new LoginWithOpenIdConnectRequest()
            {
                TitleId = TitleId,
                CreateAccount = doCreateAccount,
                IdToken = idToken,
                ConnectionId = OpenIdProvider.Id,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                {
                    GetUserAccountInfo = true
                }
            };
            PlayFabClientAPI.LoginWithOpenIdConnect(request, OnSucces, OnError);

            void OnSucces(LoginResult result)
            {
                LoginResult = result;
                onComplete?.Invoke();
            }

            void OnError(PlayFabError error)
            {
                Debug.LogErrorFormat("Error while trying to Login with OpenId: {0}", error.GenerateErrorReport());
                onFailure?.Invoke();
            }
        }

        #endregion
        
    }
}
#endif