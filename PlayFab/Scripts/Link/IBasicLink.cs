#if CREOBIT_BACKEND_PLAYFAB
using System;

namespace Creobit.Backend.Link
{
    public interface IBasicLink
    {
        void Link(bool forceRelink, Action onComplete, Action onFailure);
    }
}
#endif