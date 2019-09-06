using System;

namespace Creobit.Backend.Link
{
    public interface ILink
    {
        #region ILink

        void Link(string linkKey, Action onComplete, Action onFailure);

        void RequestLinkKey(int linkKeyLenght, Action<string> onComplete, Action onFailure);

        #endregion
    }
}
