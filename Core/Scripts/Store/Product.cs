using System;
using System.Text;

namespace Creobit.Backend.Store
{
    internal class Product : PurchasableItem, IProduct
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

        #region Product

        private Action<IProduct, Action, Action> _purchaseDelegate;

        public Product(string id, IPrice price) : base(id, price)
        {
        }

        #endregion
    }
}
