#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;

namespace Creobit.Backend.Inventory
{
    public interface ISteamItemDefinition : IItemDefinition<ISteamItemInstance>
    {
        #region ISteamItemDefinition

        InventoryDef InventoryDef
        {
            get;
        }

        #endregion
    }
}
#endif
