#if CREOBIT_BACKEND_UNITY && CREOBIT_BACKEND_GOOGLEPLAY
using System;
using UnityEngine;

namespace Creobit.Backend.Store
{
    //That class is a wrapper for json structure provided by GooglePlay native methods.
    [Serializable]
    internal struct GooglePlayJson
    {
        //Needs to have exactly this name otherwise JsonUtility.FromJson will not parse it correctly.
        [SerializeField]
        private string freeTrialPeriod;

        public int? TrialDuration
        {
            get
            {
                if (string.IsNullOrWhiteSpace(freeTrialPeriod))
                {
                    return null;
                }

                var period = freeTrialPeriod.Substring(1);
                if (!int.TryParse(period.Substring(0, period.Length - 1), out var numberOfUnits))
                {
                    return null;
                }
                var unitString = period.Substring(period.Length - 1);
                var unitDuration = GetUnitDuration(unitString);

                var result = numberOfUnits * unitDuration;
                if (result == 0)
                {
                    return null;
                }
                return result;
            }
        }

        private int GetUnitDuration(string unitString)
        {
            switch (unitString)
            {
                case "D":
                    return 1;

                case "W":
                    return 7;

                case "M":
                    return 30;

                case "Y":
                    return 365;

                default:
                    return 0;
            }
        }
    }
}
#endif