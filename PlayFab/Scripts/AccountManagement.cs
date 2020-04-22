#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using Creobit.Backend.Link;

namespace Creobit.Backend
{
    public sealed class AccountManagement : IAccountManagement
    {
        public IPlayFabAuth Auth { get; }

        public IBasicLink Link { get; }

        public AccountManagement(IPlayFabAuth auth, IBasicLink link)
        {
            Auth = auth;
            Link = link;
        }
    }
}
#endif