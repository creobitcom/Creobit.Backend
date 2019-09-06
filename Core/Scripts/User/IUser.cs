using System;

namespace Creobit.Backend.User
{
    public interface IUser
    {
        #region IUser

        string AvatarUrl
        {
            get;
        }

        string Name
        {
            get;
        }

        void Refresh(Action onComplete, Action onFailure);

        void SetAvatarUrl(string avatarUrl, Action onComplete, Action onFailure);

        void SetName(string name, Action onComplete, Action onFailure);

        #endregion
    }
}
