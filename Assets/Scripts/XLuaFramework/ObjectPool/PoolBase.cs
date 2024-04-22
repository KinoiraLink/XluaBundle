using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
namespace XLuaFramework.ObjectPool
{
    public class PoolBase : MonoBehaviour
    {
        protected float releaseTime;

        //上次释放的资源的时间 毫微秒 10^7
        protected long lastReleaseTime = 0;

        protected List<PoolObject> objects;

        public void Start()
        {
            lastReleaseTime = DateTime.Now.Ticks;
        }

        public void Init(float time)
        {
            releaseTime = time;
            objects = new List<PoolObject>();
        }

        public virtual Object Spawn(string gameName)
        {
            foreach (PoolObject po in objects)
            {
                if (po.poolObjectName == gameName)
                {
                    objects.Remove(po);
                    return po.poolObject;
                }
                
            }
            return null;
        }

        public virtual void UnSpawn(string gameObjectName,Object obj)
        {
            PoolObject po = new PoolObject(gameObjectName, obj);
            objects.Add(po);
        }

        public virtual void Release()
        {
        }

        private void Update()
        {
            if (DateTime.Now.Ticks - lastReleaseTime >= releaseTime * 10000000)
            {
                lastReleaseTime = DateTime.Now.Ticks;
                Release();
            }
        }
    }
}