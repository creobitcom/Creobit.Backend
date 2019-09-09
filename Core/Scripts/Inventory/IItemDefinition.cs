using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> BundledItemDefinitions
        {
            get;
        }

        string Id
        {
            get;
        }

        #endregion
    }
}
