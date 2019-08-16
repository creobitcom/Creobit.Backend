﻿#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class SteamAuth : ISteamAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => SteamClient.IsLoggedOn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            if (SteamClient.RestartAppIfNecessary(AppId))
            {
                if (Application.isEditor)
                {
                    var exception = new Exception("The Steam is not started!");

                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
                else
                {
                    Application.Quit();
                }

                return;
            }

            try
            {
                SteamClient.Init(AppId);

                onComplete();
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Process(exception);

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
                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region ISteamAuth

        uint ISteamAuth.AppId => AppId;

        string ISteamAuth.CreateAuthSessionTicket()
        {
            var authTicket = Steamworks.SteamUser.GetAuthSessionTicket();
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
            get;
            set;
        } = Backend.ExceptionHandler.Default;

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
