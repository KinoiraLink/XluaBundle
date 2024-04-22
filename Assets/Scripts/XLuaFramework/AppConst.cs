using XLuaFramework.Enums;

namespace XLuaFramework
{
    public class AppConst
    {
        public const string BUNDLE_EXTENSION = ".ab";
        public const string FILE_LIST_NAME = "filelist.txt";

        public static bool openLog = true;
        //热更服务器地址
        public const string RESOURCES_URL = "http://127.0.0.1/AssetBundles";
        public static GameMode gameMode = GameMode.EditorMode;
    }
}
