using System;
using System.Text;

namespace Creobit.Backend.Store
{
    internal class Product : IProduct
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ ");
            stringBuilder.Append($"{nameof(Id)}:{Id} ");
            stringBuilder.Append($"{nameof(Price)}:{Price} ");
            stringBuilder.Append($"}}");

            return stringBuilder.ToString();
        }

        #endregion
        #region IIdentifiable

        string IIdentifiable.Id => Id;

        #endregion
        #region IPurchasable

        IPrice IPurchasable.Price => Price;

        void IPurchasable.Purchase(Action onComplete, Action onFailure) => PurchaseDelegate(this, onComplete, onFailure);

        #endregion
        #region Product

        public readonly string Id;
        public readonly IPrice Price;

        private Action<IProduct, Action, Action> _purchaseDelegate;

        public Product(string id, IPrice price)
        {
            Id = id;
            Price = price;
        }

        public Action<IProduct, Action, Action> PurchaseDelegate
        {
            get => _purchaseDelegate ?? Purchase;
            set => _purchaseDelegate = value;
        }

        private void Purchase(IProduct product, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        #endregion
    }
}
