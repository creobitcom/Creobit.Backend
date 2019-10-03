namespace Creobit.Backend.Store
{
    public interface IPrice : IIdentifiable
    {
        #region IPrice

        string CurrencyCode
        {
            get;
        }

        uint Value
        {
            get;
        }

        #endregion
    }
}
