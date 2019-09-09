using System;

namespace Creobit.Backend.Inventory
{
    public interface IItem
    {
        #region IItem

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
