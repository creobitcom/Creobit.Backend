#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Creobit.Backend.Wallet
{
    public sealed class PlayFabWallet : IPlayFabWallet
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"{{");
            stringBuilder.AppendLine($"  {nameof(Currencies)}:");

            foreach (var currency in Currencies)
            {
                stringBuilder.AppendLine($"  {currency}");
            }

            stringBuilder.Append($"}}");

            return stringBuilder.ToString();
        }

        #endregion
        #region IRefreshable

        void IRefreshable.Refresh(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest()
                    {
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserVirtualCurrency = true
                        }
                    },
                    result =>
                    {
                        var infoResultPayload = result.InfoResultPayload;

                        VirtualCurrencies = infoResultPayload.UserVirtualCurrency ?? new Dictionary<string, int>();
                        VirtualCurrencyRechargeTimes = infoResultPayload.UserVirtualCurrencyRechargeTimes ?? new Dictionary<string, VirtualCurrencyRechargeTime>();

                        UpdateCurrencies();

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region IWallet

        IEnumerable<ICurrency> IWallet.Currencies => Currencies;

        #endregion
        #region IPlayFabWallet

        IEnumerable<(string CurrencyId, string PlayFabVirtualCurrencyId)> IPlayFabWallet.CurrencyMap => CurrencyMap;

        #endregion
        #region PlayFabWallet

        private IList<ICurrency> _currencies;
        private IDictionary<string, int> _virtualCurrencies;
        private IDictionary<string, VirtualCurrencyRechargeTime> _virtualCurrencyRechargeTimes;

        private IEnumerable<(string CurrencyId, string PlayFabVirtualCurrencyId)> _currencyMap;
        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        private IList<ICurrency> Currencies
        {
            get => _currencies ?? Array.Empty<ICurrency>();
            set => _currencies = value;
        }

        public IEnumerable<(string CurrencyId, string PlayFabVirtualCurrencyId)> CurrencyMap
        {
            get => _currencyMap ?? Array.Empty<(string CurrencyId, string PlayFabVirtualCurrencyId)>();
            set => _currencyMap = value;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        private IDictionary<string, int> VirtualCurrencies
        {
            get => _virtualCurrencies ?? new ReadOnlyDictionary<string, int>(new Dictionary<string, int>());
            set => _virtualCurrencies = value;
        }

        private IDictionary<string, VirtualCurrencyRechargeTime> VirtualCurrencyRechargeTimes
        {
            get => _virtualCurrencyRechargeTimes ?? new ReadOnlyDictionary<string, VirtualCurrencyRechargeTime>(new Dictionary<string, VirtualCurrencyRechargeTime>());
            set => _virtualCurrencyRechargeTimes = value;
        }

        private void Consume(ICurrency currency, uint count, Action onComplete, Action onFailure)
        {
            var playFabVirtualCurrencyId = this.FindPlayFabVirtualCurrencyId(currency.Id);

            if (playFabVirtualCurrencyId == null)
            {
                var exception = new Exception($"The PlayFabVirtualCurrencyId is not found for the CurrencyId \"{currency.Id}\"!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }

            try
            {
                PlayFabClientAPI.SubtractUserVirtualCurrency(
                    new SubtractUserVirtualCurrencyRequest()
                    {
                        Amount = Convert.ToInt32(count),
                        VirtualCurrency = playFabVirtualCurrencyId
                    },
                    result =>
                    {
                        UpdatePlayFabVirtualCurrency(result.VirtualCurrency, result.Balance);
                        UpdateCurrencies();

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        private void Grant(ICurrency currency, uint count, Action onComplete, Action onFailure)
        {
            var playFabVirtualCurrencyId = this.FindPlayFabVirtualCurrencyId(currency.Id);

            if (playFabVirtualCurrencyId == null)
            {
                var exception = new Exception($"The PlayFabVirtualCurrencyId is not found for the CurrencyId \"{currency.Id}\"!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }

            try
            {
                PlayFabClientAPI.AddUserVirtualCurrency(
                    new AddUserVirtualCurrencyRequest()
                    {
                        Amount = Convert.ToInt32(count),
                        VirtualCurrency = playFabVirtualCurrencyId
                    },
                    result =>
                    {
                        UpdatePlayFabVirtualCurrency(result.VirtualCurrency, result.Balance);

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        private void UpdateCurrencies()
        {
            Currencies = CreateCurrencies();

            List<ICurrency> CreateCurrencies()
            {
                var currencies = new List<ICurrency>();

                foreach (var (CurrencyId, PlayFabVirtualCurrencyId) in CurrencyMap)
                {
                    if (PlayFabVirtualCurrencyId == "RM")
                    {
                        continue;
                    }

                    if (!VirtualCurrencies.TryGetValue(PlayFabVirtualCurrencyId, out var count))
                    {
                        var exception = new Exception($"The PlayFabVirtualCurrency is not found for the PlayFabVirtualCurrencyId \"{PlayFabVirtualCurrencyId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var currency = Currencies
                        .FirstOrDefault(x => x.Id == CurrencyId);

                    if (currency == null)
                    {
                        currency = new Currency(CurrencyId)
                        {
                            ConsumeDelegate = Consume,
                            Count = count,
                            GrantDelegate = Grant
                        };
                    }
                    else
                    {
                        ((Currency)currency).Count = count;
                    }

                    currencies.Add(currency);
                }

                return currencies;
            }
        }

        internal void UpdatePlayFabVirtualCurrency(string virtualCurrencyId, int count)
        {
            VirtualCurrencies[virtualCurrencyId] = count;

            UpdateCurrencies();
        }

        #endregion
    }
}
#endif
