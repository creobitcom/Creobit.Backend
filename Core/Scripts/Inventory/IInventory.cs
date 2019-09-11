using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IInventory
    {
        #region IInventory

        void LoadCurrencyDefinitions(Action onComplete, Action onFailure);

        void LoadCurrencyInstances(Action onComplete, Action onFailure);

        void LoadItemDefinitions(Action onComplete, Action onFailure);

        void LoadItemInstances(Action onComplete, Action onFailure);

        #endregion
    }

    public interface IInventory<out TCurrencyDefinition, out TCurrencyInstance, out TItemDefinition, out TItemInstance> : IInventory
        where TCurrencyDefinition : ICurrencyDefinition
        where TCurrencyInstance : ICurrencyInstance
        where TItemDefinition : IItemDefinition
        where TItemInstance : IItemInstance
    {
        #region IInventory<TCurrencyDefinition, TCurrencyInstance, TItemDefinition, TItemInstance>

        IEnumerable<TCurrencyDefinition> CurrencyDefinitions
        {
            get;
        }

        IEnumerable<TCurrencyInstance> CurrencyInstances
        {
            get;
        }

        IEnumerable<TItemDefinition> ItemDefinitions
        {
            get;
        }

        IEnumerable<TItemInstance> ItemInstances
        {
            get;
        }

        #endregion
    }
}
