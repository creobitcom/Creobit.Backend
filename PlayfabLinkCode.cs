#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IChainBlock = Creobit.IChainBlock<bool>;

namespace Creobit.Backend.Link
{
    public class PlayfabLinkCode : IPlayFabLink
    {
        #region PlayfabLinkCode

        private const float DefaultAvailabilityTime = 3 * 60f;
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
            get => _availabilityTime ?? DefaultAvailabilityTime;
            set => _availabilityTime = value;
        }

        protected virtual void Restore(Action onFailure)
        {
            _customAccount.Auth.Logout(Relogin, onFailure);

            void Relogin()
            {
                _customAccount = null;
                OriginalAccount.Auth.Login(onFailure, onFailure);
            }
        }

        private void CheckLinkKeyExpirationTime(Action<bool> handler)
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
                            handler?.Invoke(false);
                            return;
                        }

                        var now = DateTime.Now;
                        var expirationTime = DateTime.Parse(record.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                        if (now > expirationTime)
                        {
                            handler?.Invoke(false);
                        }
                        else
                        {
                            handler?.Invoke(true);
                        }
                    },
                    error =>
                    {
                        handler?.Invoke(false);
                    }
                );
            }
            catch (Exception)
            {
                handler?.Invoke(false);
            }
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


        protected virtual void RequestChain(string linkKey, Action<IEnumerable<IChainBlock>> handler)
        {
            var chain = Enumerable.Empty<IChainBlock>();
            //TODO - add some syntax sugar.
            chain = chain.Append(new SimpleChainBlock(trigger => _customAccount.Auth.Login(() => trigger?.Invoke(true), () => trigger?.Invoke(false))));
            chain = chain.Append(new SimpleChainBlock(CheckLinkKeyExpirationTime));
            chain = chain.Append(new SimpleChainBlock(trigger => trigger?.Invoke(CanPerformLink())));

            handler?.Invoke(chain);
        }

        #endregion
        #region IPlayFabLink

        void ILinkCode.Link(string linkKey, Action onComplete, Action onFailure)
        {
            //TODO - that is a workaround. It should stay in place as long as we pass PlayFabAuth to specific Authentification methods.
            var playfabAuth = new PlayFabAuth(OriginalAccount.Auth.TitleId);
            _customAccount = new AccountManagement(new CustomPlayfabAuth(playfabAuth, linkKey), new CustomPlayFabLink(linkKey));

            RequestChain(linkKey, OnChainGenerated);
            void OnChainGenerated(IEnumerable<IChainBlock> chain)
            {
                //TODO - add some syntax sugar.
                chain = chain.Append(new SimpleChainBlock(trigger => OriginalAccount.Link.Link(true, () => trigger?.Invoke(true), reason => trigger?.Invoke(false))));
                //TODO - add some syntax sugar.
                chain = chain.Append(new SimpleChainBlock(trigger => _customAccount.Link.Unlink(() => trigger?.Invoke(true), () => trigger?.Invoke(true))));

                chain.Execute(OnComplete, () => Restore(onFailure));
            }

            void OnComplete()
            {
                _customAccount = null;
                onComplete?.Invoke();
            }
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