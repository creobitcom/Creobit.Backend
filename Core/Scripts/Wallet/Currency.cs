using System;
using System.Text;

namespace Creobit.Backend.Wallet
{
    internal sealed class Currency : ICurrency
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ ");
            stringBuilder.Append($"{nameof(Id)}:{Id} ");
            stringBuilder.Append($"{nameof(Count)}:{Count} ");
            stringBuilder.Append($"}}");

            return stringBuilder.ToString();
        }

        #endregion
        #region IConsumable

        void IConsumable.Consume(uint count, Action onComplete, Action onFailure) => ConsumeDelegate(this, count, onComplete, onFailure);

        #endregion
        #region ICountable

        int ICountable.Count => Count;

        #endregion
        #region IGrantable

        void IGrantable.Grant(uint count, Action onComplete, Action onFailure) => GrantDelegate(this, count, onComplete, onFailure);

        #endregion
        #region IIdentifiable

        string IIdentifiable.Id => Id;

        #endregion
        #region Currency

        private readonly string Id;

        private Action<ICurrency, uint, Action, Action> _consumeDelegate;
        private Action<ICurrency, uint, Action, Action> _grantDelegate;

        public Currency(string id)
        {
            Id = id;
        }

        public Action<ICurrency, uint, Action, Action> ConsumeDelegate
        {
            get => _consumeDelegate ?? Consume;
            set => _consumeDelegate = value;
        }

        public int Count
        {
            get;
            set;
        }

        public Action<ICurrency, uint, Action, Action> GrantDelegate
        {
            get => _grantDelegate ?? Grant;
            set => _grantDelegate = value;
        }

        private void Consume(ICurrency currency, uint count, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        private void Grant(ICurrency currency, uint count, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        #endregion
    }
}
