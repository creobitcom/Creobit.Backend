#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
{
    public sealed class CustomPlayfabAuth : PlayFabAuthDecorator
    {
        private readonly string CustomId;

        public CustomPlayfabAuth(IPlayFabAuth playFabAuth, string customId)
            : base(playFabAuth)
        {
            CustomId = customId;
        }


        protected override void Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LoginWithCustomID
            (
                new LoginWithCustomIDRequest()
                {
                    TitleId = TitleId,
                    CreateAccount = doCreateAccount,
                    CustomId = CustomId,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                    {
                        GetUserAccountInfo = true
                    },
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
    }
}
#endif