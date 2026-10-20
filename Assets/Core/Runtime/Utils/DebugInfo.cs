using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Utils
{
    public static class DebugInfo
    {
        public static string @scene = $"{SceneManager.GetActiveScene().name}";
        public static string @frame = $"{Time.frameCount}";
    }
}