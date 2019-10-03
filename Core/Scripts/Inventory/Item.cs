using Creobit.Backend.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Creobit.Backend.Inventory
{
    internal class Item : IItem
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ ");
            stringBuilder.Append($"{nameof(Id)}:{Id} ");
            stringBuilder.Append($"{nameof(Count)}:{Count} ");

            if (BundledCurrencies.Any())
            {
                stringBuilder.Append($"{nameof(BundledCurrencies)}:[ ");

                foreach (var currency in BundledCurrencies)
                {
                    stringBuilder.Append($"{currency} ");
                }

                stringBuilder.Append($"] ");
            }

            if (BundledItems.Any())
            {
                stringBuilder.Append($"{nameof(BundledItems)}:[ ");

                foreach (var item in BundledItems)
                {
                    stringBuilder.Append($"{item} ");
                }

                stringBuilder.Append($"] ");
            }

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
        #region IItem

        IEnumerable<ICurrency> IItem.BundledCurrencies => BundledCurrencies;

        IEnumerable<IItem> IItem.BundledItems => BundledItems;

        #endregion
        #region PlayFabItem

        private readonly string Id;

        private IList<ICurrency> _bundledCurrencies;
        private IList<IItem> _bundledItems;

        private Action<IItem, uint, Action, Action> _consumeDelegate;
        private Action<IItem, uint, Action, Action> _grantDelegate;

        public Item(string id)
        {
            Id = id;
        }

        public Action<IItem, uint, Action, Action> ConsumeDelegate
        {
            get => _consumeDelegate ?? Consume;
            set => _consumeDelegate = value;
        }

        public IList<ICurrency> BundledCurrencies
        {
            get => _bundledCurrencies ?? Array.Empty<ICurrency>();
            set => _bundledCurrencies = value;
        }

        public IList<IItem> BundledItems
        {
            get => _bundledItems ?? Array.Empty<IItem>();
            set => _bundledItems = value;
        }

        public int Count
        {
            get;
            set;
        }

        public Action<IItem, uint, Action, Action> GrantDelegate
        {
            get => _grantDelegate ?? Grant;
            set => _grantDelegate = value;
        }

        private void Consume(IItem item, uint count, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        private void Grant(IItem item, uint count, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        #endregion
    }
}
