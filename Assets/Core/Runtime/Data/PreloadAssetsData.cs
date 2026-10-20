using System.Collections.Generic;
using System.IO;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace AT.Core.Runtime.Data
{
    [CreateAssetMenu(menuName = "DowneyJoy/Data/Preload Assets")]
    public class PreloadAssetsData : CloneableObject
    {
        public List<AssetEntry> entries = new List<AssetEntry>();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(entries,Formatting.Indented);
        }

        public static string ToJson(List<AssetEntry> entries)
        {
            return JsonConvert.SerializeObject(entries,Formatting.Indented);
        }

        public void LoadFromJson(string json)
        {
            entries = JsonConvert.DeserializeObject<List<AssetEntry>>(json);
        }

        public static PreloadAssetsData FromJson(string json)
        {
            List<AssetEntry> entries = JsonConvert.DeserializeObject<List<AssetEntry>>(json);
            var data = CreateInstance<PreloadAssetsData>();
            data.entries = entries;
            return data;
        }
        [ContextMenu("Save To Json")]
        public void SaveToJson()
        {
            string json = ToJson();
            string path = ContentDefinitions.GetPreloadAssetsPath();
            File.WriteAllText(path, json);
        }
    }
}