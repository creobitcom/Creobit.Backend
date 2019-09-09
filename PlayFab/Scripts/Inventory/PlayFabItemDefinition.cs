#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabItemDefinition : IPlayFabItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> IItemDefinition.BundledItemDefinitions
            => (IEnumerable<(IItemDefinition ItemDefinition, uint Count)>)_bundledItemDefinitions
            ?? Array.Empty<(IItemDefinition ItemDefinition, uint Count)>();

        string IItemDefinition.Id => Id;

        #endregion
        #region IPlayFabItemDefinition

        CatalogItem IPlayFabItemDefinition.CatalogItem => CatalogItem;

        #endregion
        #region PlayFabItemDefinition

        internal readonly string Id;
        internal readonly CatalogItem CatalogItem;

        private List<(IItemDefinition ItemDefinition, uint Count)> _bundledItemDefinitions;

        internal PlayFabItemDefinition(string id, CatalogItem catalogItem)
        {
            Id = id;
            CatalogItem = catalogItem;
        }

        internal void AddBundledItemDefinition(IItemDefinition itemDefinition, uint count)
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

        #endregion
    }
}
#endif
