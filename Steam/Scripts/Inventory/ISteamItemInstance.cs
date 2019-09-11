#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;

namespace Creobit.Backend.Inventory
{
    public interface ISteamItemInstance : IItemInstance<ISteamItemDefinition>
    {
        #region ISteamItemInstance

        InventoryItem InventoryItem
        {
            get;
        }

        #endregion
    }
}
#endif
