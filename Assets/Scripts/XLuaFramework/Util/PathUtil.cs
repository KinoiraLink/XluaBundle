using System.IO;
using UnityEngine;
using XLuaFramework.Enums;

namespace XLuaFramework.Util
{
    public class PathUtil
    {
        //Assets 目录
        public static readonly string AssetsPath = Application.dataPath;
        //打 Bundle 输入路径
        public static readonly string BuildResourcesPath = Path.Combine(AssetsPath, "BuildResources");
        //打 Bundle 输出路径
        public static readonly string BuildOutPath = Application.streamingAssetsPath;
        
        //只读目录
        public static readonly string ReadPath = Application.streamingAssetsPath;
        
        //可读写目录
        public static readonly string ReadWritePath = Application.persistentDataPath;
        
        //lua文件路径
        public static readonly string LuaPath = "Assets/BuildResources/LuaScripts";
        
        //bundle资源路径
        public static string BundleResourcePath 
        {
            get
            {
                if (AppConst.gameMode == GameMode.UpdateMode)
                    return ReadWritePath;
                return ReadPath;
            }

        }

        /// <summary>
        /// 获取 Unity 的相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUnityPath(string path) => string.IsNullOrEmpty(path) ? string.Empty : path.Substring(path.IndexOf("Assets"));

        /// <summary>
        /// 获取标准路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetStandardPath(string path)=> string.IsNullOrEmpty(path) ? string.Empty :path.Trim().Replace('\\', '/');


        public static string GetLuaPath(string name) =>
            $"Assets/BuildResources/LuaScripts/{name}.lua.bytes";
        
        public static string GetUIPath(string name) =>
            $"Assets/BuildResources/UI/Prefabs/{name}.prefab";
        
        public static string GetMusicPath(string name) =>
            $"Assets/BuildResources/Audio/Music/{name}";
        
        public static string GetSoundPath(string name) =>
            $"Assets/BuildResources/Audio/Sound/{name}";
        
        public static string GetEffectPath(string name) =>
            $"Assets/BuildResources/Effect/Prefabs/{name}.prefab";

        public static string GetModelPath(string name) =>
            $"Assets/BuildResources/Model/Prefabs/{name}.prefab";
        
        public static string GetSpritePath(string name) =>
            $"Assets/BuildResources/Sprites/{name}";
        
        public static string GetScenePath(string name) =>
            $"Assets/BuildResources/Scenes/{name}.unity";
    }
}

