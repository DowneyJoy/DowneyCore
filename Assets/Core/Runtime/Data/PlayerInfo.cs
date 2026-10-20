using System;
using Newtonsoft.Json;

namespace AT.Core.Runtime.Data
{
    [JsonObject]
    public class PlayerInfo
    {
        [JsonProperty]public string id;
        [JsonProperty]public string name;

        public PlayerInfo(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
        
        public static string GenerateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}