#if CREOBIT_BACKEND_APPSTORE && CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_UNITY && UNITY_STANDALONE_OSX
using System;
using UnityEngine;

namespace Creobit.Backend.Store
{
    [Serializable]
    public struct Receipt
    {
        [SerializeField]
        public string Payload;
    }

}
#endif