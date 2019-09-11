#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabItemInstance : IItemInstance<IPlayFabItemDefinition>
    {
        #region IPlayFabItemInstance

        ItemInstance NativeItemInstance
        {
            get;
        }

        #endregion
    }
}
#endif
