using System;

namespace Creobit.Backend.Inventory
{
    public interface ICurrencyInstance
    {
        #region ICurrencyInstance

        uint Count
        {
            get;
        }

        void Consume(uint count, Action onComplete, Action onFailure);

        #endregion
    }

    public interface ICurrencyInstance<out TCurrencyDefinition> : ICurrencyInstance
        where TCurrencyDefinition : ICurrencyDefinition
    {
        #region ICurrencyInstance<TCurrencyDefinition>

        TCurrencyDefinition CurrencyDefinition
        {
            get;
        }

        #endregion
    }
}
