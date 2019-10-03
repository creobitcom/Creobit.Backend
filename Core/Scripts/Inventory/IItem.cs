using Creobit.Backend.Wallet;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IItem : IConsumable, ICountable, IGrantable, IIdentifiable
    {
        #region IItem

        IEnumerable<ICurrency> BundledCurrencies
        {
            get;
        }

        IEnumerable<IItem> BundledItems
        {
            get;
        }

        #endregion
    }
}
