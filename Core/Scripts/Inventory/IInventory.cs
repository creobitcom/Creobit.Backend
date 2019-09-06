using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IInventory
    {
        #region IInventory

        IEnumerable<IDefinition> Definitions
        {
            get;
        }

        IEnumerable<IItem> Items
        {
            get;
        }

        void LoadDefinitions(Action onComplete, Action onFailure);

        void LoadItems(Action onComplete, Action onFailure);

        #endregion
    }
}
