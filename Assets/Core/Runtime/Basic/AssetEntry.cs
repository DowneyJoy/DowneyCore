using System;
using Object = UnityEngine.Object;

namespace AT.Core.Runtime.Basic
{
    [Serializable]
    public class AssetEntry : SerializedKeyValuePair<string, Object>
    {
        public AssetType type;
        
        public AssetEntry(string key, Object value, AssetType type) : base(key,value)
        {
            this.type = type;
        }
    }
}