#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamPlayFabInventory : IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, IItemDefinition, IItemInstance<IItemDefinition>>
    {
        #region IInventory

        void IInventory.LoadCurrencyDefinitions(Action onComplete, Action onFailure) => PlayFabInventory.LoadCurrencyDefinitions(onComplete, onFailure);

        void IInventory.LoadCurrencyInstances(Action onComplete, Action onFailure) => PlayFabInventory.LoadCurrencyInstances(onComplete, onFailure);

        void IInventory.LoadItemDefinitions(Action onComplete, Action onFailure)
        {
            var errorCount = 0;
            var invokeCount = 2;

            PlayFabInventory.LoadItemDefinitions(
                () =>
                {
                    invokeCount -= 1;

                    Handle();
                },
                () =>
                {
                    errorCount += 1;
                    invokeCount -= 1;

                    Handle();
                });

            SteamInventory.LoadItemDefinitions(
                () =>
                {
                    invokeCount -= 1;

                    Handle();
                },
                () =>
                {
                    errorCount += 1;
                    invokeCount -= 1;

                    Handle();
                });

            void Handle()
            {
                if (invokeCount != 0)
                {
                    return;
                }

                if (errorCount > 0)
                {
                    onFailure();
                }
                else
                {
                    onComplete();
                }
            }
        }

        void IInventory.LoadItemInstances(Action onComplete, Action onFailure)
        {
            var errorCount = 0;
            var invokeCount = 2;

            PlayFabInventory.LoadItemInstances(
                () =>
                {
                    invokeCount -= 1;

                    Handle();
                },
                () =>
                {
                    errorCount += 1;
                    invokeCount -= 1;

                    Handle();
                });

            SteamInventory.LoadItemInstances(
                () =>
                {
                    invokeCount -= 1;

                    Handle();
                },
                () =>
                {
                    errorCount += 1;
                    invokeCount -= 1;

                    Handle();
                });

            void Handle()
            {
                if (invokeCount != 0)
                {
                    return;
                }

                if (errorCount > 0)
                {
                    onFailure();
                }
                else
                {
                    onComplete();
                }
            }
        }

        #endregion
        #region IInventory<IItemDefinition, IItemInstance<IItemDefinition>>

        IEnumerable<ICurrencyDefinition> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, IItemDefinition, IItemInstance<IItemDefinition>>.CurrencyDefinitions
        {
            get
            {
                foreach (var currencyDefinition in PlayFabInventory.CurrencyDefinitions)
                {
                    yield return currencyDefinition;
                }

                foreach (var currencyDefinition in SteamInventory.CurrencyDefinitions)
                {
                    yield return currencyDefinition;
                }
            }
        }

        IEnumerable<ICurrencyInstance<ICurrencyDefinition>> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, IItemDefinition, IItemInstance<IItemDefinition>>.CurrencyInstances
        {
            get
            {
                foreach (var currencyInstance in PlayFabInventory.CurrencyInstances)
                {
                    yield return currencyInstance;
                }

                foreach (var currencyInstance in SteamInventory.CurrencyInstances)
                {
                    yield return currencyInstance;
                }
            }
        }

        IEnumerable<IItemDefinition> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, IItemDefinition, IItemInstance<IItemDefinition>>.ItemDefinitions
        {
            get
            {
                foreach (var itemDefinition in PlayFabInventory.ItemDefinitions)
                {
                    yield return itemDefinition;
                }

                foreach (var itemDefinition in SteamInventory.ItemDefinitions)
                {
                    yield return itemDefinition;
                }
            }
        }

        IEnumerable<IItemInstance<IItemDefinition>> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, IItemDefinition, IItemInstance<IItemDefinition>>.ItemInstances
        {
            get
            {
                foreach (var itemInstance in PlayFabInventory.ItemInstances)
                {
                    yield return itemInstance;
                }

                foreach (var itemInstance in SteamInventory.ItemInstances)
                {
                    yield return itemInstance;
                }
            }
        }

        #endregion
        #region SteamPlayFabInventory

        public readonly IPlayFabInventory PlayFabInventory;
        public readonly ISteamInventory SteamInventory;

        public SteamPlayFabInventory(IPlayFabInventory playFabInventory, ISteamInventory steamInventory)
        {
            PlayFabInventory = playFabInventory;
            SteamInventory = steamInventory;
        }

        #endregion
    }
}
#endif
