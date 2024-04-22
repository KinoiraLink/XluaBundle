using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLuaFramework.DataInfo;
using XLuaFramework.Enums;
using XLuaFramework.Util;
using Object = UnityEngine.Object;

namespace XLuaFramework.Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        //缓存 bundleInfo
        private Dictionary<string, BundleInfo> _bundleInfos;
    
        //缓存已解压Bundle资源
        private Dictionary<string, BundleData> _assetBundles;

        private void Awake()
        {
            _bundleInfos = new Dictionary<string, BundleInfo>();
            _assetBundles = new Dictionary<string, BundleData>();
        }

        /// <summary>
        /// 解析版本文件
        /// </summary>
        public void ParseVersionFile()
        {
            //读取文件
            string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FILE_LIST_NAME);
            string[] data = File.ReadAllLines(url);
        
            //解析文件
            for (int i = 0; i < data.Length; i++)
            {
                BundleInfo bundleInfo = new BundleInfo();
                string[] info = data[i].Split('|');
                bundleInfo.assetsName = info[0];
                bundleInfo.bundleName = info[1];
                bundleInfo.dependences = new List<string>(info.Length - 2);
            
                for(int j = 2; j < info.Length; j++)
                {
                    bundleInfo.dependences.Add(info[j]);
                }
                _bundleInfos.Add(bundleInfo.assetsName,bundleInfo);

                if (info[0].IndexOf("LuaScripts", StringComparison.Ordinal) > 0)
                {
                    Manager.LuaManager.luaNames.Add(info[0]);
                }
            }
        }
#if UNITY_EDITOR
        private void EditorLoadAsset(string assetName,Action<Object> callback = null)
        {
            Debug.Log("EditorMode");
            Object obj = AssetDatabase.LoadAssetAtPath(assetName, typeof(Object));
            if(obj == null)
                Debug.LogError($"{assetName} is not exist");
            callback?.Invoke(obj);
        }
#endif

    
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">回调</param>
        private void LoadAsset(string assetName, Action<Object> callback)
        {
#if UNITY_EDITOR
            if (AppConst.gameMode == GameMode.EditorMode)
            {
                EditorLoadAsset(assetName,callback);
            }
#endif
            if (AppConst.gameMode == GameMode.UpdateMode || AppConst.gameMode == GameMode.PackageBundle)
            {
                StartCoroutine(LoadBundleAsync(assetName,callback));

            }
/*#if UNITY_EDITOR
        EditorLoadAsset(assetName,callback);
#else
        StartCoroutine(LoadBundleAsync(assetName,callback));
#endif*/
        }
        
        
        

        //加载Lua
        public void LoadLua(string assetName, Action<Object> callback) => LoadAsset(assetName, callback);
    
        //加载UI
        public void LoadUI(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetUIPath(assetName), callback);
    
        //加载音乐
        public void LoadMusic(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetMusicPath(assetName), callback);
    
        //加载音效
        public void LoadSound(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetSoundPath(assetName), callback);
    
        //加载特效
        public void LoadEffect(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetEffectPath(assetName), callback);
    
        //加载场景
        public void LoadScene(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetScenePath(assetName), callback);
    
        //加载Sprite
        public void LoadSprite(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetSpritePath(assetName), callback);
        
        //加载模型
        public void LoadPrefab(string assetName, Action<Object> callback) => LoadAsset(PathUtil.GetModelPath(assetName), callback);

        public static void UnloadBundle(Object obj)
        {
            AssetBundle ab = obj as AssetBundle;
            if (ab != null) ab.Unload(true);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        private IEnumerator LoadBundleAsync(string assetName, Action<Object> callback = null)
        {
            string bundleName = _bundleInfos[assetName].bundleName;
            string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
            List<string> dependence = _bundleInfos[assetName].dependences;

            BundleData bundle = GetBundle(bundleName);
            if(bundle == null)
            {
                //先查对象池
                
                Object obj = Manager.PoolManager.Spawn("AssetBundle",bundleName);
                if (obj != null)
                {
                    AssetBundle ab = obj as AssetBundle;
                    bundle = new BundleData(ab);
                }
                else
                {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                    yield return request;
                    bundle = new BundleData(request.assetBundle);
                }
                _assetBundles.Add(bundleName,bundle);
            }
            
            if (dependence is { Count: > 0 })
            {
                for (int i = 0; i < dependence.Count; i++)
                {
                    yield return LoadBundleAsync(dependence[i]);
                }
            }
            if (assetName.EndsWith(".unity"))
            {
                if (callback != null) callback.Invoke(null);
                yield break;
            }
            
            if(callback == null)
                yield break;
            
            //加载资源
            AssetBundleRequest bundleRequest = bundle.bundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            //返回资源
            if (bundleRequest != null)
            {
                callback.Invoke(bundleRequest.asset);
            }
        }

        private BundleData GetBundle(string bundleName)
        {
            if (_assetBundles.TryGetValue(bundleName, out BundleData bundle))
            {
                bundle.count++;
                return bundle;
            }

            return null;
        }

        public void MinusBundleCount(string assetName)
        {
            string bundleName = _bundleInfos[assetName].bundleName;

            MinusOneBundleCount(bundleName);

            List<string> dependences = _bundleInfos[assetName].dependences;
            if (dependences != null)
            {
                foreach (string dependence in dependences)
                {
                    string bn = _bundleInfos[dependence].bundleName;
                    MinusOneBundleCount(bn);
                }
            }
        }

        private void MinusOneBundleCount(string bundleName)
        {
            if (_assetBundles.TryGetValue(bundleName, out BundleData bundle))
            {
                if (bundle.count > 0)
                {
                    bundle.count--;
                    Debug.Log("bundle引用计数：" + bundleName + "count:" + bundle.count);
                }

                if (bundle.count <= 0)
                {
                    Debug.Log("放入bundle对象池:" + bundleName);
                    Manager.PoolManager.UnSpawn("AssetBundle", bundleName,bundle.bundle);
                    _assetBundles.Remove(bundleName);
                }
            }
        }
    }
}
