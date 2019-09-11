using System;

namespace Creobit.Backend.Inventory
{
    public interface IItemInstance
    {
        #region IItemInstance

        uint Count
        {
            get;
        }

        void Consume(uint count, Action onComplete, Action onFailure);

        #endregion
    }

    public interface IItemInstance<out TItemDefinition> : IItemInstance
        where TItemDefinition : IItemDefinition
    {
        #region IItemInstance<TItemDefinition>

        TItemDefinition ItemDefinition
        {
            get;
        }

        #endregion
    }
}
