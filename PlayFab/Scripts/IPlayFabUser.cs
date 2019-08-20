#if CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend
{
    public interface IPlayFabUser : IUser
    {
        #region IPlayFabUser

        string Id
        {
            get;
        }

        bool IsNewlyCreated
        {
            get;
        }

        #endregion
    }
}
#endif
