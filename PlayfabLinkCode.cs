#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend;
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creobit.Backend.Link
{
    public sealed class PlayfabLinkCode : IPlayFabLink
    {
        #region PlayfabLinkCode

        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private readonly IAccountManagement OriginalAccount;

        private IAccountManagement _customAccount;

        private float? _availabilityTime;

        public PlayfabLinkCode(IAccountManagement originalAccount)
        {
            OriginalAccount = originalAccount;
        }

        public float AvailabilityTime
        {
            get => _availabilityTime ?? 180f;
            set => _availabilityTime = value;
        }

        private void Restore(Action onFailure)
        {
            _customAccount.Auth.Logout(Relogin, onFailure);

            void Relogin()
            {
                _customAccount = null;
                OriginalAccount.Auth.Login(onFailure, onFailure);
            }
        }

        private void OnCustomLoginPerformed(Action onComplete, Action onFailure)
        {
            _customAccount.Link.Unlink(() => CheckLinkKeyExpirationTime(onComplete, onFailure), onFailure);
        }

        private void CheckLinkKeyExpirationTime(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetUserData
                (
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

                        if (!data.TryGetValue(LinkKeyExpirationTime, out var record))
                        {
                            Restore(onFailure);
                            return;
                        }

                        var now = DateTime.Now;
                        var expirationTime = DateTime.Parse(record.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                        if (now > expirationTime)
                        {
                            Restore(onFailure);
                        }
                        else
                        {
                            LinkAccounts(onComplete, onFailure);
                        }
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
        }

        private void LinkAccounts(Action onComplete, Action onFailure)
        {
            if (!CanPerformLink())
            {
                Restore(onFailure);
                return;
            }

            OriginalAccount.Link.Link(true, onComplete, reason => onFailure?.Invoke());
        }

        private bool CanPerformLink()
        {
            return OriginalAccount.Link.CanLink(_customAccount.Auth.LoginResult);
        }

        private string CreateLinkKey(int lenght)
        {
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var stringBuilder = new StringBuilder(6);

            for (var i = 0; i < lenght; ++i)
            {
                var character = characters[random.Next(0, characters.Length)];

                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        #endregion
        #region IPlayFabLink

        void ILinkCode.Link(string linkKey, Action onComplete, Action onFailure)
        {
            //TODO - that is a workaround. It should stay in place as long as we pass PlayFabAuth to specific Authentification methods.
            var playfabAuth = new PlayFabAuth(OriginalAccount.Auth.TitleId);
            _customAccount = new AccountManagement(new CustomPlayfabAuth(playfabAuth, linkKey), new CustomPlayFabLink(linkKey));
            _customAccount.Auth.Login(() => OnCustomLoginPerformed(onComplete, onFailure), onFailure);
        }

        void ILinkCode.RequestLinkKey(int linkKeyLenght, Action<(string LinkKey, DateTime LinkKeyExpirationTime)> onComplete, Action onFailure)
        {
            var linkKey = CreateLinkKey(linkKeyLenght);
            IBasicLink link = new CustomPlayFabLink(linkKey);
            link.Link(false, WriteLinkKeyExpirationTime, reason => onFailure?.Invoke());

            void WriteLinkKeyExpirationTime()
            {
                try
                {
                    var now = DateTime.Now;
                    var expirationTime = now + TimeSpan.FromSeconds(AvailabilityTime);

                    PlayFabClientAPI.UpdateUserData(
                        new UpdateUserDataRequest()
                        {
                            Data = new Dictionary<string, string>()
                            {
                                { LinkKeyExpirationTime, expirationTime.ToString("O", CultureInfo.InvariantCulture) }
                            },
                            Permission = UserDataPermission.Private
                        },
                        result =>
                        {
                            onComplete((linkKey, expirationTime));
                        },
                        error =>
                        {
                            onFailure();
                        });
                }
                catch (Exception)
                {
                    onFailure();
                }
            }
        }

        #endregion
    }
}
#endif