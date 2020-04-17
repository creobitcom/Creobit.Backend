using System;

namespace Creobit.Backend.Auth
{
    public interface IOpenIdProvider
    {
        string Id { get; }

        void RequestToken(Action<string> onObtained);
    }
}