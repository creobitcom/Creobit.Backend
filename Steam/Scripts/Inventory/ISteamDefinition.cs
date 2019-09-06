#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;

namespace Creobit.Backend.Inventory
{
    public interface ISteamDefinition : IDefinition
    {
        #region ISteamDefinition

        InventoryDef InventoryDef
        {
            get;
        }

        #endregion
    }
}
#endif
