namespace Creobit.Backend
{
    public interface ISteamAuth : IAuth
    {
        #region ISteamAuth

        uint AppId
        {
            get;
        }

        string CreateAuthSessionTicket();

        void DestroyAuthSessionTicket(string authSessionTicket);

        #endregion
    }
}