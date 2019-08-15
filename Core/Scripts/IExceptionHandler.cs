using System;

namespace Creobit.Backend
{
    public interface IExceptionHandler
    {
        #region IExceptionHandler

        void Process(Exception exception);

        #endregion
    }
}