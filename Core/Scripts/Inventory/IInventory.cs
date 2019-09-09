using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IInventory
    {
        #region IInventory

        IEnumerable<IItemDefinition> ItemDefinitions
        {
            get;
        }

        IEnumerable<IItem> Items
        {
            get;
        }

        void LoadItemDefinitions(Action onComplete, Action onFailure);

        void LoadItems(Action onComplete, Action onFailure);

        #endregion
    }
}
