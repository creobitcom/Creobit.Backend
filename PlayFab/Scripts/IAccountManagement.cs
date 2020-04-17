#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using Creobit.Backend.Link;

namespace Creobit.Backend
{
    public interface IAccountManagement
    {
        IPlayFabAuth Auth { get; }
        IBasicLink Link { get; }
    }
}
#endif