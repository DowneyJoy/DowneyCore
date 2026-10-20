using System;
using Unity.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Basic
{
    public class LoadingInfo
    {
        public event Action OnLoadBegin;
        public event Action OnLoadEnd;
        public event Action<float> OnLoadProgress;
        
        public float Progress {get; private set;}
        public string CurrentLoad {get; private set;}
        public bool IsLoading {get; private set;}

        public LoadingInfo(string task, Action OnLoadBegin, Action OnLoadEnd, Action<float> OnLoadProgress)
        {
            CurrentLoad = task;
            this.OnLoadBegin = OnLoadBegin;
            this.OnLoadEnd = OnLoadEnd;
            this.OnLoadProgress = OnLoadProgress; 
        }
        
        public void StartLoading(string id)
        {
            if (IsLoading)
            {
                Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Load(Start) at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Refused, Reason: Task in queue.");
                return;
            }
            OnLoadBegin?.Invoke();
            Progress = 0;

            IsLoading = true;
        }

        public void StopLoading(string id)
        {
            if (!IsLoading)
            {
                if (IsLoading)
                {
                    Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Load(Stop) at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Refused, Reason: No Exist Task.");
                    return;
                }
            }
            OnLoadEnd?.Invoke();
            Progress = 0;

            IsLoading = false;
        }

        public void Report(string id,float progress)
        {
            if (!IsLoading)
            {
                if (IsLoading)
                {
                    Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Load(Report) at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Refused, Reason: No Exist Task.");
                    return; 
                }
            }
            Progress = progress;
            OnLoadProgress?.Invoke(progress);
        }
    }
}