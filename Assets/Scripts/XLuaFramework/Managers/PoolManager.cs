using System.Collections.Generic;
using UnityEngine;
using XLuaFramework.ObjectPool;
using Object = UnityEngine.Object;

namespace XLuaFramework.Managers
{
    public class PoolManager : MonoBehaviour
    {
        private Transform _poolParent;

        private Dictionary<string, PoolBase> _pools;

        private void Awake()
        {
            _poolParent = this.transform.parent.Find("Pool");
            _pools = new Dictionary<string, PoolBase>();
        }


        private void CreatePool<T>(string poolName, float releaseTime)
            where T : PoolBase
        {
            if (!_pools.TryGetValue(poolName, out PoolBase pool))
            {
                GameObject go = new GameObject(poolName);
                go.transform.SetParent(_poolParent);
                pool = go.AddComponent<T>();
                pool.Init(releaseTime);
                _pools.Add(poolName,pool);
            }
        }

        //创建游戏对象池
        public void CreateGameObjectPool(string poolName, float releaseTime)
        {
            CreatePool<GameObjectPool>(poolName,releaseTime);
        }

        //创建资源池
        public void CreateAssetPool(string poolName, float releaseTime)
        {
            CreatePool<AssetPool>(poolName,releaseTime);
        }

        //取出对象
        public Object Spawn(string poolName, string assetName)
        {
            //先取池子
            if (_pools.TryGetValue(poolName, out PoolBase pool))
            {
                //再取对象
                return pool.Spawn(assetName);
            }

            return null;
        }
        //回收对象
        public void UnSpawn(string poolName, string assetName, Object asset)
        {
            if (_pools.TryGetValue(poolName, out PoolBase pool))
            {
                pool.UnSpawn(assetName,asset);
            }
        }
    }
}