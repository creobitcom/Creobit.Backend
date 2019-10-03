using System;
using UnityEngine;

namespace Creobit.Backend
{
    internal sealed class ExceptionHandler : IExceptionHandler
    {
        #region IExceptionHandler

        void IExceptionHandler.Process(Exception exception)
        {
            Debug.LogError(exception);
        }

        #endregion
        #region ExceptionHandler

        public static readonly IExceptionHandler Default = new ExceptionHandler();

        #endregion
    }
}
