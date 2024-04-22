using System;
using UnityEngine;

namespace XLuaFramework.ObjectPool
{
    public class AssetPool : PoolBase
    {
        public override void Release()
        {
            base.Release();
            foreach (PoolObject item in objects)
            {
                if (DateTime.Now.Ticks - item.lastUseTime.Ticks >= releaseTime * 10000000)
                {
                    Debug.Log("AssetPool release time:" + DateTime.Now + "unload asset bundle:" + item.poolObjectName);
                    Managers.ResourcesManager.UnloadBundle(item.poolObject);
                    objects.Remove(item);
                    Release();//递归释放，防止因为Remove对象迭代器出错
                    return;
                }
            }
        }
    }
}