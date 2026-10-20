using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEditor;
using UnityEditor.Compilation;

namespace AT.Core.Editor
{
    public static class CompilationTime
    {
        private static double _startTime;
        private static double _endTime;
        private static double _duration;
        
        static CompilationTime(){}
        
        [InitializeOnLoadMethod]
        static void Init()
        {
            InitLogger();

            CompilationPipeline.compilationStarted += Start;
            CompilationPipeline.compilationFinished += End;
        }

        static void InitLogger()
        {
            LoggerConfig cfg = new LoggerConfig();
            cfg = cfg.SyncMode.FullAsync();
            cfg = cfg.CaptureStacktrace(true);
            cfg = cfg.OutputTemplate("{Timestamp}[{Level}]{Message}");
            cfg = cfg.WriteTo.UnityDebugLog(outputTemplate:"<color=orange>[{Level}]</color> {Message}",captureStackTrace:true);

            Log.Logger = cfg.CreateLogger();
        }

        static void Start(object o)
        {
            _startTime = EditorApplication.timeSinceStartup;
            
            Log.Info($"[Compiler] Compilation Started.");
        }
        
        static void End(object o)
        {
            _endTime = EditorApplication.timeSinceStartup;
            _duration = _endTime - _startTime;
            
            Log.Info($"[Compiler] Compilation Finished. Duration: {_duration:0.000} s");
        }
    }
}