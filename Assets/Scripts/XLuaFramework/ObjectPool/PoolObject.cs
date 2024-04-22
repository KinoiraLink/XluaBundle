using System;
using Object = UnityEngine.Object;

namespace XLuaFramework.ObjectPool
{
    public class PoolObject
    {
        public Object poolObject;

        public string poolObjectName;

        public DateTime lastUseTime;

        public PoolObject(string poolObjectName, Object obj)
        {
            this.poolObjectName = poolObjectName;
            poolObject = obj;
            lastUseTime = DateTime.Now;
        }
    }
}