using System.Collections;
using System.IO;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.Manager;
using AT.Core.Runtime.Utils;
using Unity.Logging;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace AT.Core
{
    [RequireComponent(typeof(VideoPlayer),typeof(AudioSource))]
    public class BootMovie : AutoSingleton<BootMovie>,ISingleton,IPersistence
    {
        private VideoPlayer _player;
        private Camera _cam;
        private AudioSource _audio;
        
        void ISingleton.Single(string id)
        {
            string url = ContentDefinitions.GetBootMoviePath();

            if (!File.Exists(url))
            {
                Log.Info($"Object: {id}, Type: {typeof(BootMovie).FullName},Action:Init(Auto) at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, Fatal：File Missing, File:{url}");
                Bootstrap.ForceCrash();
                return;
            }
            InitComponents(url);
        }

        void InitComponents(string url)
        {
            InitPlayer(url);
            InitCamera();
            InitAudio();
            
            FinishInit();
        }
        
        void InitPlayer(string url)
        {
            _player = GetComponent<VideoPlayer>();
            
            _player.url = url;
            _player.renderMode = VideoRenderMode.CameraNearPlane;
            _player.playOnAwake = false;
            _player.skipOnDrop = false;
            _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        }

        void InitCamera()
        {
            _cam = new GameObject("video_cam").AddComponent<Camera>();
            _cam.transform.SetParent(transform);
            
            _cam.orthographic = true;
            _cam.orthographicSize = 5;
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = Color.black;
        }
        void InitAudio()
        {
            _audio = GetComponent<AudioSource>();
            
            _audio.playOnAwake = false;
            _audio.loop = false;
            _audio.volume = 1f;
            _audio.spatialBlend = 0;
        }

        void FinishInit()
        {
            _player.targetCamera = _cam;
            _player.SetTargetAudioSource(0, _audio);
        }

        public IEnumerator OnMovie()
        {
            yield return new WaitForEndOfFrame();
            Log.Info($"Object: {id}, Type: {typeof(BootMovie).FullName}, Action: OnMovie, Phase: Prepare Video, Video: {_player.url} at @scene: {SceneManager.GetActiveScene().name}");
            _player.Prepare();
            while (!_player.isPrepared)
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            Log.Info($"Object: {id}, Type: {typeof(BootMovie).FullName}, Action: OnMovie, Phase: Play Video, Video: {_player.url} at @scene: {SceneManager.GetActiveScene().name}");
            
            _player.Play();
            while (_player.isPlaying)
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            Log.Info($"Object: {id}, Type: {typeof(BootMovie).FullName}, Action: OnMovie, Phase: Clean Up Trash, Video: {_player.url}, Expected Duration: {_player.time:0.000} at @scene: {SceneManager.GetActiveScene().name}");
            _player.Stop();
            yield return  new WaitForEndOfFrame();
            yield return CleanUp();
        }

        IEnumerator CleanUp()
        {
            Destroy(gameObject);
            yield return null;
        }
    }
}