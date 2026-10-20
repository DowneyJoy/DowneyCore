using AT.Core.Runtime.Basic;
using UnityEngine;
using Unity.Logging;

namespace AT.Core.Runtime.Utils
{
    public class MonoSingleton<T> : BasicBehaviour where T : MonoSingleton<T>
    {
        public static T instance => _instance;
        private static T _instance;

        private void Awake()
        {
            OnInit();
            if (instance != null && instance != this)
            {
                Log.Error($"Object: {id},Type: {typeof(T).FullName}, Action: Singleton Init(Mono), State: Error, Reason: Instance already exists,Callback: Destroy Object, Target:{id}, Type: {typeof(T).FullName}");
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            Log.Info($"Object: {id}, Type: {typeof(T).FullName}, Action: Singleton Init(Mono), State: Success");
            
            (_instance as ISingleton)?.Single(id);
            (_instance as IPersistence)?.Init(_instance.gameObject);
        }
    }
}