using System;
using UnityEngine;

namespace AT.Core.Runtime.Basic
{
    public class BasicObject : ScriptableObject,IBasic
    {
        public string id { get; private set; }

        private void OnEnable()
        {
            OnInit();
        }

        public void OnInit()
        {
            #if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                id = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            }
            #endif
        }
    }
}