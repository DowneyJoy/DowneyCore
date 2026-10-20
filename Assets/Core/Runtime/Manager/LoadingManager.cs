using System;
using System.Collections.Generic;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Utils;
using Unity.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Manager
{
    public class LoadingManager : AutoSingleton<LoadingManager>, ISingleton,IPersistence
    {
        private readonly List<LoadingInfo> _loadingTasks = new List<LoadingInfo>();

        void ISingleton.Single(string id)
        {
            _loadingTasks.Clear();
        }

        public void StartLoading(string task,Action OnStart = null,Action OnFinish = null,Action<float> OnProgress = null)
        {
            var info = _loadingTasks.Find(t => t.CurrentLoad.Equals(task,StringComparison.OrdinalIgnoreCase));

            if (info == null)
            {
                info = new LoadingInfo(task,OnStart,OnFinish,OnProgress);
                _loadingTasks.Add(info);
            }
            info.StartLoading(id);
        }

        public void StopLoading(string task)
        {
            var info = _loadingTasks.Find(t => t.CurrentLoad.Equals(task,StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                info.StopLoading(id);
                _loadingTasks.Remove(info);
            }
        }

        public void Report(string task,float progress)
        {
            var info = _loadingTasks.Find(t => t.CurrentLoad.Equals(task,StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                info.Report(id,progress);
            }
        }
    }
}