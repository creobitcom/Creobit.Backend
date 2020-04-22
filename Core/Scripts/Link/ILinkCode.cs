using System;

namespace Creobit.Backend.Link
{
    public interface ILinkCode
    {
        #region ILink

        void Link(string linkKey, Action onComplete, Action<LinkCodeError> onFailure);

        void RequestLinkKey(int linkKeyLenght, Action<(string LinkKey, DateTime LinkKeyExpirationTime)> onComplete, Action onFailure);

        #endregion
    }
}
