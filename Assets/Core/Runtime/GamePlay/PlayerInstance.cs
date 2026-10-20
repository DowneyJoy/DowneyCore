using AT.Core.Runtime.Basic;
using UnityEngine;

namespace AT.Core.Runtime.GamePlay
{
    public class PlayerInstance
    {
        public string id { get; set; }
        public string name { get; set; }
        
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 acceleration { get; set; }
        
        public IPawn pawn { get; set; }

        public PlayerInstance(string id, string name, IPawn pawn)
        {
            this.id = id;
            this.name = name;
            this.position = Vector3.zero;
            this.rotation = Quaternion.identity;
            this.velocity = Vector3.zero;
            this.acceleration = Vector3.zero;
            this.pawn = pawn;
        }
    }
}