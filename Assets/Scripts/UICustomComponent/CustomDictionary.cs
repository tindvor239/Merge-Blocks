using System;
using System.Collections.Generic;

namespace UnityEngine.CustomComponents
{
    [Serializable]
    public class CustomDictionary<TKey, TValue>
    {
        [SerializeField]
        private TKey key;
        [SerializeField]
        private TValue value;

        #region Properties
        public TValue Value { get => value; }
        public TKey Key { get => key; }
        #endregion
    }
}