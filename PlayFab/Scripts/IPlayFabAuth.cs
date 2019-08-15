using PlayFab.ClientModels;

namespace Creobit.Backend
{
    public interface IPlayFabAuth : IAuth
    {
        #region IPlayFabAuth

        LoginResult LoginResult
        {
            get;
            set;
        }

        string TitleId
        {
            get;
        }

        #endregion
    }
}