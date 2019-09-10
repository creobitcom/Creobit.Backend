#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabItem : IItem<IPlayFabItemDefinition>
    {
        #region IPlayFabItem

        ItemInstance ItemInstance
        {
            get;
        }

        #endregion
    }
}
#endif
