using System;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Utils;
using UnityEngine;

namespace AT.Core.Runtime.GamePlay
{
    public class PlayerStarter : MonoSingleton<PlayerStarter>,ISingleton
    {
        public static Vector3 position { get; private set; } = Vector3.zero;
        public static Quaternion rotation { get; private set; } = Quaternion.identity;
        public bool faceRight = true;

        public void Single(string id)
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        private void OnDrawGizmos()
        {
            Vector3 center = position + Vector3.up;
            Vector3 right =  rotation * Vector3.right;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(center, new Vector3(1f, 2f, 1f));
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(center, right.normalized*1.5f);
        }
    }
}