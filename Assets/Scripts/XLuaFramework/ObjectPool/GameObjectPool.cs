using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XLuaFramework.ObjectPool
{
    public class GameObjectPool : PoolBase
    {
        public override Object Spawn(string gameName)
        {
            Object obj =  base.Spawn(gameName);
            if (obj == null)
                return null;
            GameObject go = obj as GameObject;
            if (go != null) go.SetActive(true);
            return obj;
        }

        public override void UnSpawn(string gameObjectName, Object obj)
        {
            GameObject go = obj as GameObject;
            if (go != null)
            {
                go.SetActive(false);
                go.transform.SetParent(this.transform, false);
            }

            base.UnSpawn(gameObjectName,obj);
        }

        public override void Release()
        {
            base.Release();
            foreach (PoolObject item in objects)
            {
                if (DateTime.Now.Ticks - item.lastUseTime.Ticks >= releaseTime * 10000000)
                {
                    Debug.Log("GameObjectPool release time:" + DateTime.Now);
                    Destroy(item.poolObject);
                    Managers.Manager.Resources.MinusBundleCount(item.poolObjectName);
                    objects.Remove(item);
                    Release();//递归释放，防止因为Remove对象迭代器出错
                    return;
                }
            }
        }
    }
}