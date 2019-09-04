#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabDefinition : IPlayFabDefinition
    {
        #region IDefinition

        IEnumerable<(IDefinition Definition, uint Count)> IDefinition.BundledDefinitions => (IEnumerable<(IDefinition Definition, uint Count)>)_bundledDefinitions
            ?? Array.Empty< (IDefinition Definition, uint Count)>();

        string IDefinition.Id => Id;

        #endregion
        #region IPlayFabDefinition

        CatalogItem IPlayFabDefinition.CatalogItem => CatalogItem;

        #endregion
        #region PlayFabDefinition

        internal readonly string Id;
        internal readonly CatalogItem CatalogItem;

        private List<(IDefinition Definition, uint Count)> _bundledDefinitions;

        internal PlayFabDefinition(string id, CatalogItem catalogItem)
        {
            Id = id;
            CatalogItem = catalogItem;
        }

        internal void AddBundledDefinition(IDefinition definition, uint count)
        {
            if (_bundledDefinitions == null)
            {
                _bundledDefinitions = new List<(IDefinition Definition, uint Count)>();
            }

            for (var i = 0; i < _bundledDefinitions.Count; ++i)
            {
                if (_bundledDefinitions[i].Definition == definition)
                {
                    _bundledDefinitions[i] = (definition, _bundledDefinitions[i].Count + count);

                    return;
                }
            }

            _bundledDefinitions.Add((definition, count));
        }

        #endregion
    }
}
#endif
