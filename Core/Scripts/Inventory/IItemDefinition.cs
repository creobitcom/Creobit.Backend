using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(ICurrencyDefinition CurrencyDefinition, uint Count)> BundledCurrencyDefinitions
        {
            get;
        }

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> BundledItemDefinitions
        {
            get;
        }

        string Id
        {
            get;
        }

        #endregion
    }

    public interface IItemDefinition<out TItemInstance> : IItemDefinition
        where TItemInstance : IItemInstance
    {
        #region IItemDefinition<TItemInstance>

        void Grant(uint count, Action<IEnumerable<TItemInstance>> onComplete, Action onFailure);

        #endregion
    }
}
