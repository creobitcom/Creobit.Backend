using System;

namespace Creobit.Backend.Inventory
{
    public interface IItem
    {
        #region IItem

        IDefinition Definition
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
