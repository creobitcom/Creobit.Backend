using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Creobit.Backend.Tests.PlayMode
{
    public abstract class BasePlayFabTests
    {
        #region BasePlayFabTests

        protected static readonly IEnumerable<(string CurrencyId, string PlayFabVirtualCurrencyId)>
            PlayFabWalletCurrencyMap =
                new (string CurrencyId, string PlayFabVirtualCurrencyId)[]
                {
                    ("coins", "CC"),
                    ("gold", "GG")
                };

        protected static readonly IEnumerable<(string ItemId, string PlayFabItemId)> PlayFabInventoryItemMap =
            new (string ItemId, string PlayFabItemId)[]
            {
                // Items
                ("bow", "Bow"),
                ("knife", "Knife"),
                ("potion", "Potion"),
                ("shield", "Shield"),
                ("sword", "Sword"),
                // Bundles
                ("archer_pack", "ArcherPack"),
                ("swordsman_pack", "SwordsmanPack")
            };

        protected static readonly IEnumerable<(string PriceId, string PlayFabVirtualCurrencyId)> PlayFabStorePriceMap =
            new (string PriceId, string PlayFabVirtualCurrencyId)[]
            {
                // Virtual Currencies
                ("coins", "CC"),
                ("gold", "GG"),
                // Real Currencids
                ("money", "RM")
            };

        protected static readonly IEnumerable<(string ProductId, string PlayFabItemId)> PlayFabStoreProductMap =
            new (string ProductId, string PlayFabItemId)[]
            {
                // Items
                ("potion", "Potion"),
                // Bundles
                ("archer_pack", "ArcherPack"),
                ("swordsman_pack", "SwordsmanPack")
            };

        protected string _titleId = "12513";

        protected IEnumerator PlayFabWhileHandler(Action<Action, Action> action)
        {
            var result = false;
            action?.Invoke(() => { result = true; }, () => { Assert.Fail($"On failure {action}"); });

            while (!result)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion
    }
}