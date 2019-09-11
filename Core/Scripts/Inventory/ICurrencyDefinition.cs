using System;

namespace Creobit.Backend.Inventory
{
    public interface ICurrencyDefinition
    {
        #region ICurrencyDefinition

        string Id
        {
            get;
        }

        #endregion
    }

    public interface ICurrencyDefinition<out TCurrencyInstance> : ICurrencyDefinition
        where TCurrencyInstance : ICurrencyInstance
    {
        #region ICurrencyDefinition<TCurrencyInstance>

        void Grant(uint count, Action<TCurrencyInstance> onComplete, Action onFailure);

        #endregion
    }
}
