using AT.Core.Runtime.Basic;
using Unity.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Utils
{
    public class AutoSingleton<T>:BasicBehaviour where T : AutoSingleton<T>
    {
        public static T instance
        {
            get
            {
                if(_instance != null) return _instance;
                T ins;
                T[] vessels = GameObject.FindObjectsOfType<T>();
                string result = "Get";
                if(vessels.Length == 1) ins = vessels[0];
                else if (vessels.Length > 1)
                {
                    ins = vessels[0];
                    for (int i = 1; i < vessels.Length; i++)
                    {
                        GameObject.Destroy(vessels[i].gameObject);
                    }

                    result = "Delete Duplicates";
                }
                else
                {
                    GameObject vessel = new GameObject(typeof(T).Name.Replace(" ","_").ToLower().Trim());
                    ins = vessel.AddComponent<T>();
                    result = "Create";
                }
                _instance = ins;
                Log.Info($"Object: {_instance.GetInstanceID()}, Type: {typeof(T).FullName}，Action: Get Instance, Result: {result} at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
                return _instance;
            }
        }
        private static T _instance;
        

        public void Init()
        {
            OnInit();
            if (instance != null && instance != this)
            {
                Log.Error($"Object: {id},Type: {typeof(T).FullName}, Action: Singleton Init(Auto), State: Error, Reason: Instance already exists,Callback: Destroy Object, Target:{id}, Type: {typeof(T).FullName}");
                Destroy(gameObject);
                return;
            }
            Log.Info($"Object: {id}, Type: {typeof(T).FullName}, Action: Singleton Init(Auto), State: Success");
            
            (this as ISingleton)?.Single(id);
            (this as IPersistence)?.Init(_instance.gameObject);
        }
    }
}