using System;
using AT.Core.Runtime.Manager;
using UnityEngine;

namespace AT.Core.Runtime.Definitions
{
    public static class ContentDefinitions
    {
        public const string BOOT_MOVIE_FILE = "movie.mp4";
        public const string PRELOAD_ASSETS_FILE = "preload.data";
        public const string INPUT_ACTIONS_FILE = "input.data";
        public const string PAWN_DATA_FILE = "character.data";
        public const string PLAYER_INFO_KEY = "PL_INFO";
        public const string PAWN_ADDRESS = "assets/prefabs/pawn.prefab";

        public static string GetInputActionsPath()
        {
            return PathManager.GetFile(INPUT_ACTIONS_FILE, Application.persistentDataPath);
        }

        public static string GetBootMoviePath()
        {
            return PathManager.GetFile(BOOT_MOVIE_FILE, Application.streamingAssetsPath);
        }
        public static string GetPreloadAssetsPath()
        {
            return PathManager.GetFile(PRELOAD_ASSETS_FILE, Application.streamingAssetsPath);
        }

        public static string GetLogFilePath()
        {
            return PathManager.GetFile($"at_{DateTime.Now:yy-MMM-dd}.log", Application.persistentDataPath,
                PathDefinitions.LOGS_FOLDER);
        }

        public static string GetPawnDataPath()
        {
            return PathManager.GetFile(PAWN_DATA_FILE,Application.persistentDataPath,PathDefinitions.DATA_FOLDER);
        }
    }
}