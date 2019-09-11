#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    internal sealed class PlayFabItemDefinition : IPlayFabItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(ICurrencyDefinition CurrencyDefinition, uint Count)> IItemDefinition.BundledCurrencyDefinitions
            => (IEnumerable<(ICurrencyDefinition CurrencyDefinition, uint Count)>)_bundledCurrencyDefinitions
            ?? Array.Empty<(ICurrencyDefinition CurrencyDefinition, uint Count)>();

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> IItemDefinition.BundledItemDefinitions
            => (IEnumerable<(IItemDefinition ItemDefinition, uint Count)>)_bundledItemDefinitions
            ?? Array.Empty<(IItemDefinition ItemDefinition, uint Count)>();

        string IItemDefinition.Id => Id;

        #endregion
        #region IItemDefinition<TItemInstance>

        void IItemDefinition<IPlayFabItemInstance>.Grant(uint count, Action<IEnumerable<IPlayFabItemInstance>> onComplete, Action onFailure)
            => Grant(this, count, onComplete, onFailure);

        #endregion
        #region IPlayFabItemDefinition

        CatalogItem IPlayFabItemDefinition.NativeCatalogItem => CatalogItem;

        #endregion
        #region PlayFabItemDefinition

        public readonly string Id;
        public readonly CatalogItem CatalogItem;

        private List<(ICurrencyDefinition CurrencyDefinition, uint Count)> _bundledCurrencyDefinitions;
        private List<(IItemDefinition ItemDefinition, uint Count)> _bundledItemDefinitions;

        public PlayFabItemDefinition(string id, CatalogItem catalogItem)
        {
            Id = id;
            CatalogItem = catalogItem;
        }

        public void AddBundledCurrencyDefinition(ICurrencyDefinition currencyDefinition, uint count)
        {
            if (_bundledCurrencyDefinitions == null)
            {
                _bundledCurrencyDefinitions = new List<(ICurrencyDefinition CurrencyDefinition, uint Count)>();
            }

            for (var i = 0; i < _bundledCurrencyDefinitions.Count; ++i)
            {
                if (_bundledCurrencyDefinitions[i].CurrencyDefinition == currencyDefinition)
                {
                    _bundledCurrencyDefinitions[i] = (currencyDefinition, _bundledCurrencyDefinitions[i].Count + count);

                    return;
                }
            }

            _bundledCurrencyDefinitions.Add((currencyDefinition, count));
        }

        public void AddBundledItemDefinition(IItemDefinition itemDefinition, uint count)
        {
            if (_bundledItemDefinitions == null)
            {
                _bundledItemDefinitions = new List<(IItemDefinition ItemDefinition, uint Count)>();
            }

            for (var i = 0; i < _bundledItemDefinitions.Count; ++i)
            {
                if (_bundledItemDefinitions[i].ItemDefinition == itemDefinition)
                {
                    _bundledItemDefinitions[i] = (itemDefinition, _bundledItemDefinitions[i].Count + count);

                    return;
                }
            }

            _bundledItemDefinitions.Add((itemDefinition, count));
        }

        public Action<PlayFabItemDefinition, uint, Action<IEnumerable<PlayFabItemInstance>>, Action> Grant
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
