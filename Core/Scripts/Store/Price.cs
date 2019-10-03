using System.Text;

namespace Creobit.Backend.Store
{
    internal class Price : IPrice
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ ");
            stringBuilder.Append($"{nameof(Id)}:{Id} ");
            stringBuilder.Append($"{nameof(CurrencyCode)}:{CurrencyCode} ");
            stringBuilder.Append($"{nameof(Value)}:{Value} ");
            stringBuilder.Append($"}}");

            return stringBuilder.ToString();
        }

        #endregion
        #region IIdentifiable

        string IIdentifiable.Id => Id;

        #endregion
        #region IPrice

        string IPrice.CurrencyCode => CurrencyCode;

        uint IPrice.Value => Value;

        #endregion
        #region Price

        public readonly string Id;
        public readonly string CurrencyCode;
        public readonly uint Value;

        public Price(string id, string currencyCode, uint value)
        {
            Id = id;
            CurrencyCode = currencyCode;
            Value = value;
        }

        #endregion
    }
}
