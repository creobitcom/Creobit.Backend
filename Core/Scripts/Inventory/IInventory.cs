using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IInventory : IRefreshable
    {
        #region IInventory

        IEnumerable<IItem> Items
        {
            get;
        }

        #endregion
    }
}
