#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;

namespace Creobit.Backend.Inventory
{
    public interface ISteamItem : IItem
    {
        #region ISteamItem

        InventoryItem InventoryItem
        {
            get;
        }

        #endregion
    }
}
#endif
