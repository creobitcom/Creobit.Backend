#if CREOBIT_BACKEND_UNITY && CREOBIT_BACKEND_IOS
using System;
using UnityEngine;

namespace Creobit.Backend.Store
{
    //That class is a wrapper for json structure provided by Apple native methods.
    [Serializable]
    internal struct AppleJson
    {
        //Needs to have exactly this name otherwise JsonUtility.FromJson will not parse it correctly.
        [SerializeField]
        private string numberOfUnits;
        //Needs to have exactly this name otherwise JsonUtility.FromJson will not parse it correctly.
        [SerializeField]
        private string unit;

        public int? TrialDuration
        {
            get
            {
                if (!int.TryParse(numberOfUnits, out var number))
                {
                    return null;
                }

                var unitDuration = GetUnitDuration();
                var result = number * unitDuration;
                if (result == 0)
                {
                    return null;
                }

                return result;
            }
        }

        private int GetUnitDuration()
        {
            switch (unit)
            {
                case "0":
                    return 1;

                case "1":
                    return 7;

                case "2":
                    return 30;

                case "3":
                    return 365;

                default:
                    return 0;
            }
        }
    }
}
#endif