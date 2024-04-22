
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLuaFramework.Behaviour;

namespace XLuaFramework.Managers
{
    public class MySceneManager : MonoBehaviour
    {
        private string _logicName = "[SceneLogic]";

        private void Awake()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }



        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public void SetActive(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 叠加加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="luaName">场景lua</param>
        public void LoadScene(string sceneName,string luaName)
        {
            Manager.Resources.LoadScene(sceneName, _ =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));//叠加式
            });
            
        }
        
        /// <summary>
        /// 切换加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="luaName">场景lua</param>
        public void ChangeScene(string sceneName,string luaName)
        {
            Manager.Resources.LoadScene(sceneName, _ =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));//切换是
            });
            
        }


        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public void UnLoadSceneAsync(string sceneName)
        {
            StartCoroutine(UnLoadScene(sceneName));
        }

        
        IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
        {
            if (IsLoadedScene(sceneName))
                yield break;

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
            async.allowSceneActivation = true;//场景加载完是否跳转场景。
            yield return async;//这里不会返回场景对象

            Scene scene = SceneManager.GetSceneByName(sceneName);
            GameObject go = new GameObject(_logicName);
            //移动对象到新场景
            SceneManager.MoveGameObjectToScene(go,scene);
            SceneLogic logic = go.AddComponent<SceneLogic>();
            logic.sceneName = sceneName;
            logic.Init(luaName);
            logic.OnEnter();
        }

        private bool IsLoadedScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }
        
        private IEnumerator UnLoadScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogError("scene not isLoaded");
                yield break;
            }
            SceneLogic logic = GetSceneLogic(scene);
            logic?.OnQuit();
            AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
            yield return async;
        }

        private SceneLogic GetSceneLogic(Scene scene)
        {
            GameObject[] gameObjects = scene.GetRootGameObjects();
            foreach (var go in gameObjects)
            {
                if (String.Compare(go.name, _logicName, StringComparison.Ordinal) == 0)
                {
                    SceneLogic logic = go.GetComponent<SceneLogic>();
                    return logic;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 场景切换的回调
        /// </summary>
        /// <param name="oldScene"></param>
        /// <param name="newScene"></param>
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (!oldScene.isLoaded || !newScene.isLoaded)
                return;

            SceneLogic logic1 = GetSceneLogic(oldScene);
            SceneLogic logic2 = GetSceneLogic(newScene);
            
            logic1?.InActive();
            logic2?.OnActive();
        }
        
    }
}