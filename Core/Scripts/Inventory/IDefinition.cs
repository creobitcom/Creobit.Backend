using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IDefinition
    {
        #region IDefinition

        IEnumerable<(IDefinition Definition, uint Count)> BundledDefinitions
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
