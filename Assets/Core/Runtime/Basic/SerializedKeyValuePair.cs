using System;
using Newtonsoft.Json;

namespace AT.Core.Runtime.Basic
{
    [Serializable]
    public class SerializedKeyValuePair<TKey,TValue> where TKey : notnull where TValue : notnull
    {
        public TKey key;
        [JsonIgnore]public TValue value;
        public SerializedKeyValuePair() {}
        public SerializedKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}