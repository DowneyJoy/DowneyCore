using System;
using Unity.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Basic
{
    public class BasicBehaviour : MonoBehaviour,IBasic
    {
        public string id {get; private set;}
        
        // Call At ObjectManager,TODO
        public void OnInit()
        {
            if (!string.IsNullOrEmpty(id))
            {
                Log.Warning($"Object: {id},Action:{GetType().Name}.{nameof(OnInit)} at @scene:{SceneManager.GetActiveScene().name} at @frame:{Time.frameCount},State: Failed,Reason: Multiply Init Request.");
                return;
            }
            id = Guid.NewGuid().ToString();
            Log.Info($"Object: {id},Action:{GetType().Name}.{nameof(OnInit)} at @scene:{SceneManager.GetActiveScene().name} at @frame:{Time.frameCount},State: Success");
        }
    }
}