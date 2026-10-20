using System;
using AT.Core.Runtime.Basic;

namespace AT.Core.Runtime.Utils
{
    public class Singleton<T> : IBasic where T : Singleton<T>,new()
    {
        public static T instance
        {
            get
            {
                if(_instance != null)
                    return _instance;
                _instance = new T();
                return _instance;
            }
        }

        private static T _instance;
        
        public string id {get; private set;}

        public void Init()
        {
            (this as IBasic).OnInit();
            (this as ISingleton)?.Single(this.id);
        }

        void IBasic.OnInit()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}