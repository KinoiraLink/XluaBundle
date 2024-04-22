using System.Collections;
using UnityEngine;

namespace Tests
{
     class LoadTest : MonoBehaviour
    {
        IEnumerator Start ()
        {
            //加载请求 包
            AssetBundleCreateRequest requestPrefab = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/prefabs/loadtestprefab.prefab.ab");
            yield return requestPrefab;

            //加载请求 文件
            AssetBundleRequest bundleRequestPrefabs = requestPrefab.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefabs/LoadTestPrefab.prefab");
            yield return bundleRequestPrefabs;

            //加载请求 包
            AssetBundleCreateRequest requestImage = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/background.png.ab");
            yield return requestImage;



            //实例化
            GameObject go = Instantiate(bundleRequestPrefabs.asset) as GameObject;
            go.transform.SetParent(transform);
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
        }
    }
}

