#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using PlayFab.ClientModels;
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamPlayFabInventory : IPlayFabInventory, ISteamInventory
    {
        #region IInventory

        IEnumerable<IItem> IInventory.Items
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

        IEnumerable<IItemDefinition> IInventory.ItemDefinitions
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

        #endregion
        #region IPlayFabInventory

        string IPlayFabInventory.CatalogVersion => PlayFabInventory.CatalogVersion;

        IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> IPlayFabInventory.ItemDefinitionMap => PlayFabInventory.ItemDefinitionMap;

        GetCatalogItemsResult IPlayFabInventory.GetCatalogItemsResult => PlayFabInventory.GetCatalogItemsResult;

        GetUserInventoryResult IPlayFabInventory.GetUserInventoryResult => PlayFabInventory.GetUserInventoryResult;

        #endregion
        #region ISteamInventory

        InventoryDef[] ISteamInventory.InventoryDefs => SteamInventory.InventoryDefs;

        InventoryItem[] ISteamInventory.InventoryItems => SteamInventory.InventoryItems;

        IEnumerable<(string ItemDefinitionId, int SteamDefId)> ISteamInventory.ItemDefinitionMap => SteamInventory.ItemDefinitionMap;

        #endregion
        #region SteamPlayFabInventory

        private readonly IPlayFabInventory PlayFabInventory;
        private readonly ISteamInventory SteamInventory;

        public SteamPlayFabInventory(IPlayFabInventory playFabInventory, ISteamInventory steamInventory)
        {
            PlayFabInventory = playFabInventory;
            SteamInventory = steamInventory;
        }

        #endregion
    }
}
#endif
