using System.IO;

namespace AT.Core.Runtime.Manager
{
    public static class PathManager
    {
        public static string GetPath(params string[] paths)
        {
            string path = Path.Combine(paths).Replace('\\', '/');
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetFile(string fileName, params string[] paths)
        {
            string path = GetPath(paths);
            string fullPath = Path.Combine(path, fileName).Replace('\\','/');
            
            return fullPath;
        }
    }
}