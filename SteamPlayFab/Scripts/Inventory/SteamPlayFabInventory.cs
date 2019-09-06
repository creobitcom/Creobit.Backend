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

        IEnumerable<IDefinition> IInventory.Definitions
        {
            get
            {
                foreach (var definition in PlayFabInventory.Definitions)
                {
                    yield return definition;
                }

                foreach (var definition in SteamInventory.Definitions)
                {
                    yield return definition;
                }
            }
        }

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

        void IInventory.LoadDefinitions(Action onComplete, Action onFailure)
        {
            var errorCount = 0;
            var invokeCount = 2;

            PlayFabInventory.LoadDefinitions(
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

            SteamInventory.LoadDefinitions(
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
        #region IPlayFabInventory

        string IPlayFabInventory.CatalogVersion => PlayFabInventory.CatalogVersion;

        IEnumerable<(string DefinitionId, string PlayFabItemId)> IPlayFabInventory.DefinitionMap => PlayFabInventory.DefinitionMap;

        GetCatalogItemsResult IPlayFabInventory.GetCatalogItemsResult => PlayFabInventory.GetCatalogItemsResult;

        GetUserInventoryResult IPlayFabInventory.GetUserInventoryResult => PlayFabInventory.GetUserInventoryResult;

        #endregion
        #region ISteamInventory

        IEnumerable<(string DefinitionId, int SteamDefId)> ISteamInventory.DefinitionMap => SteamInventory.DefinitionMap;

        InventoryDef[] ISteamInventory.InventoryDefs => SteamInventory.InventoryDefs;

        InventoryItem[] ISteamInventory.InventoryItems => SteamInventory.InventoryItems;

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
