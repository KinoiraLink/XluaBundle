using UnityEngine;

namespace XLuaFramework.Util
{
    public static class Log
    {
        public static void Info(string msg)
        {
            if (!AppConst.openLog)
                return;
            Debug.Log(msg);
        }

        public static void Warning(string msg)
        {
            if (!AppConst.openLog)
                return;
            Debug.LogWarning(msg);
        }

        public static void Error(string msg)
        {
            if (!AppConst.openLog)
                return;
            Debug.LogError(msg);
        }
    }
}