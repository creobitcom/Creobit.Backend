using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IInventory
    {
        #region IInventory

        void LoadItemDefinitions(Action onComplete, Action onFailure);

        void LoadItems(Action onComplete, Action onFailure);

        #endregion
    }

    public interface IInventory<out TItemDefinition, out TItem> : IInventory
        where TItemDefinition : IItemDefinition
        where TItem : IItem<TItemDefinition>
    {
        #region IInventory<TItemDefinition, TItem>

        IEnumerable<TItemDefinition> ItemDefinitions
        {
            get;
        }

        IEnumerable<TItem> Items
        {
            get;
        }

        #endregion
    }
}
