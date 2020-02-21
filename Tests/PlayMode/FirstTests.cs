using System;
using Creobit.Backend.Wallet;
using NUnit.Framework;

namespace Creobit.Backend.Tests.PlayMode
{
    public class UserTests
    {
        [Test]
        public void CurrencyCount_SetCount10_EqualsCount10()
        {
            var currency = new Currency(String.Empty);
            currency.Count = 10;

            Assert.IsTrue(currency.Count == 10);
        }
    }
}