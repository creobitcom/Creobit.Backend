using System;

namespace Creobit.Backend.Inventory
{
    public interface IItem
    {
        #region IItem

        int? RemainingUses
        {
            get;
        }

        void Consume(int count, Action onComplete, Action onFailure);

        #endregion
    }

    public interface IItem<out TItemDefinition> : IItem
        where TItemDefinition : IItemDefinition
    {
        #region IItem<TItemDefinition>

        IItemDefinition ItemDefinition
        {
            get;
        }

        #endregion
    }
}
