#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UApplication = UnityEngine.Application;

namespace Creobit.Backend.Auth
{
    public sealed class SteamAuth : ISteamAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => SteamClient.IsLoggedOn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            if (RestartAppIfNecessary && SteamClient.RestartAppIfNecessary(AppId))
            {
                if (UApplication.isEditor)
                {
                    ExceptionHandler.Process(new SteamIsNotRunningException());

                    onFailure();
                }
                else
                {
                    UApplication.Quit();
                }

                return;
            }

            try
            {
                SteamClient.Init(AppId);

                onComplete();
            }
            catch
            {
                ExceptionHandler.Process(new SteamIsNotRunningException());

                onFailure();
            }
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            try
            {
                DestroyAuthTickets();
                SteamClient.Shutdown();

                onComplete();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region ISteamAuth

        uint ISteamAuth.AppId => AppId;

        string ISteamAuth.CreateAuthSessionTicket()
        {
            var authTicket = SteamUser.GetAuthSessionTicket();
            var stringBuilder = new StringBuilder();

            foreach (var item in authTicket.Data)
            {
                stringBuilder.Append(item.ToString("X2"));
            }

            var authSessionTicket = stringBuilder.ToString();

            AuthTickets[authSessionTicket] = authTicket;

            return authSessionTicket;
        }

        void ISteamAuth.DestroyAuthSessionTicket(string authSessionTicket)
        {
            if (AuthTickets.TryGetValue(authSessionTicket, out var authTicket))
            {
                AuthTickets.Remove(authSessionTicket);
                authTicket.Dispose();
            }
        }

        #endregion
        #region SteamAuth

        private readonly uint AppId;
        private readonly Dictionary<string, AuthTicket> AuthTickets = new Dictionary<string, AuthTicket>();

        private IExceptionHandler _exceptionHandler;

        public SteamAuth(uint appId)
        {
            AppId = appId;
        }

        ~SteamAuth()
        {
            DestroyAuthTickets();
            SteamClient.Shutdown();
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public bool RestartAppIfNecessary
        {
            get;
            set;
        } = true;

        private void DestroyAuthTickets()
        {
            foreach (var authTicket in AuthTickets.Values)
            {
                authTicket.Dispose();
            }

            AuthTickets.Clear();
        }

        #endregion
    }
}
#endif
