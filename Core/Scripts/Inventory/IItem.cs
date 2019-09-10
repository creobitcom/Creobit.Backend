using System;

namespace Creobit.Backend.Inventory
{
    public interface IItem<out TItemDefinition>
        where TItemDefinition : IItemDefinition
    {
        #region IItem<TItemDefinition>

        IItemDefinition ItemDefinition
        {
            get;
        }

        int? RemainingUses
        {
            get;
        }

        void Consume(int count, Action onComplete, Action onFailure);

        #endregion
    }
}
