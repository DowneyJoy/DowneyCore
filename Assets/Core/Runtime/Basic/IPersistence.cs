using UnityEngine;

namespace AT.Core.Runtime.Basic
{
    public interface IPersistence
    {
        void Init(GameObject owner)
        {
            GameObject.DontDestroyOnLoad(owner);
        }
    }
}