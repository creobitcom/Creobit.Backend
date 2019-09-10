#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamPlayFabInventory : IInventory<IItemDefinition, IItem<IItemDefinition>>
    {
        #region IInventory

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

        void IInventory.LoadItems(Action onComplete, Action onFailure)
        {
            var errorCount = 0;
            var invokeCount = 2;

            PlayFabInventory.LoadItems(
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

            SteamInventory.LoadItems(
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
        #region IInventory<IItemDefinition, IItem<IItemDefinition>>

        IEnumerable<IItem<IItemDefinition>> IInventory<IItemDefinition, IItem<IItemDefinition>>.Items
        {
            get
            {
                foreach (var item in PlayFabInventory.Items)
                {
                    yield return item;
                }

                foreach (var item in SteamInventory.Items)
                {
                    yield return item;
                }
            }
        }

        IEnumerable<IItemDefinition> IInventory<IItemDefinition, IItem<IItemDefinition>>.ItemDefinitions
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
