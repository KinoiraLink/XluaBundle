using UnityEngine;

namespace XLuaFramework.DataInfo
{
    public class BundleData
    {
        public AssetBundle bundle;

        //计数器
        public int count;

        public BundleData(AssetBundle ab)
        {
            bundle = ab;
            count = 1;
        }
    }
}