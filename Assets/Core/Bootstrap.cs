using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.GamePlay;
using AT.Core.Runtime.Manager;
using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AT.Core
{
    public static class Bootstrap
    {
        private static string _appId;
        
        private static CoroutineHandler _coroutineHandle = null;
        private static BootMovie _movie = null;
        private static AssetsManager _assetsManager = null;
        private static LoadingManager _loadingManager = null;
        private static PawnManager _pawnManager = null;
        private static Player _player = null;

        private static Coroutine _coroutine =  null;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void SkipUnityLogo()
        {
            if(Application.isEditor) return;

            Task.Run((() =>
            {
                SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
            }));
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            InitApplication();
            InitLogger();
            GetSysComponents();
            InitSysComponents();
            StartInitFlow();
        }

        static void InitApplication()
        {
            _appId = Guid.NewGuid().ToString();
        }

        static void InitLogger()
        {
            LoggerConfig cfg = new LoggerConfig();
            string file = ContentDefinitions.GetLogFilePath();
            cfg = cfg.SyncMode.FullAsync();
            cfg = cfg.CaptureStacktrace(true);
            cfg = cfg.OutputTemplate("{Timestamp}[{Level}]{Message}");
            cfg = cfg.WriteTo.UnityDebugLog(outputTemplate:"<color=orange>[{Level}]</color> {Message}",captureStackTrace:true).WriteTo.File(file,outputTemplate:"{Timestamp}[{Level}]{Message}",captureStackTrace:true);

            Log.Logger = cfg.CreateLogger();
            Log.Info($"Object: {_appId}, Type: {typeof(Bootstrap).FullName}, Action: Init, Phase: Init Logger at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
        }

        static void GetSysComponents()
        {
            Log.Info($"Object: {_appId}, Type: {typeof(Bootstrap).FullName}, Action: Get System Components at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
            _coroutineHandle = CoroutineHandler.instance;
            _movie = BootMovie.instance;
            _assetsManager = AssetsManager.instance;
            _loadingManager = LoadingManager.instance;
            _pawnManager =  PawnManager.instance;
            _player = Player.instance;
        }

        static void InitSysComponents()
        {
            Log.Info($"Object: {_appId}, Type: {typeof(Bootstrap).FullName}, Action: Init System Components at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
            _coroutineHandle.Init();
            _movie.Init();
            _assetsManager.Init();
            _loadingManager.Init();
            _pawnManager.Init();
            _player.Init();
            // CoroutineHandler.instance.Init();
            // BootMovie.instance.Init();
            // AssetsManager.instance.Init();
            // LoadingManager.instance.Init();
            // PawnManager.instance.Init();
            // Player.instance.Init();
        }

        static void StartInitFlow()
        {
            Log.Info($"Object: {_appId}, Type: {typeof(Bootstrap).FullName}, Action: Init Game Flow at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
            _coroutine = _coroutineHandle?.StartCoroutine(InitFlow());
        }

        static IEnumerator InitFlow()
        {
            yield return new WaitForEndOfFrame();
            yield return _movie.OnMovie();
            yield return _assetsManager.PreloadAssets();
            yield return new WaitForEndOfFrame();
            _player.Join();
        }

        static void OnApplicationQuit()
        {
            Log.Info($"Object: {_appId}, Type: {typeof(Bootstrap).FullName}, Action: Quit Application at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
            if(_coroutine != null)
                _coroutineHandle.StopCoroutine(_coroutine);
        }

        public static void QuitGame(int exitCode = 0)
        {
            OnApplicationQuit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit(exitCode);
        }

        public static void ForceCrash()
        {
#if !UNITY_EDITOR
                UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
#endif
            Bootstrap.QuitGame(1);
        }
    }
}