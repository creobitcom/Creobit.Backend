﻿using System.Collections.Generic;

namespace Creobit.Backend
{
    public interface IPlayFabStore : IStore
    {
        #region IPlayFabStore

        string CatalogVersion
        {
            get;
        }

        string StoreId
        {
            get;
        }

        // VirtualCurrency - PlayFab
        IEnumerable<(string CurrencyId, string VirtualCurrency)> CurrencyMap
        {
            get;
        }

        // ItemId - PlayFab
        IEnumerable<(string ProductId, string ItemId)> ProductMap
        {
            get;
        }

        #endregion
    }
}