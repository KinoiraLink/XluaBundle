using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
using Directory = System.IO.Directory;
using File = System.IO.File;
using XLua.LuaDLL;
using XLuaFramework.Enums;
using XLuaFramework.Util;

namespace XLuaFramework.Managers
{
    public class LuaManager : MonoBehaviour
    {
        //lua文件名
        public List<string> luaNames = new List<string>();
        
        //缓存lua脚本内容
        private Dictionary<string, byte[]> _luaScripts;

        public LuaEnv luaEnv;
        
        
        public void Init()
        {
            luaEnv = new LuaEnv();
            
            luaEnv.AddBuildin("rapidjson", Lua.LoadRapidJson);
            luaEnv.AddLoader(Loader);
            _luaScripts = new Dictionary<string, byte[]>();  
            if(AppConst.gameMode != GameMode.EditorMode)
                LoadLuaScripts();
            else
            {
#if UNITY_EDITOR
                EditorLoadLuaScript();
#endif
                
            }
        }

        public void StartLua(string luaName)
        {
            luaEnv.DoString($"require'{luaName}'");
        }

        private byte[] Loader(ref string luaName)
        {
            return GetLuaScript(luaName);
        }

        public byte[] GetLuaScript(string luaName)
        {
            luaName = luaName.Replace(".", "/");
            string fileName = PathUtil.GetLuaPath(luaName);
            
            if(!_luaScripts.TryGetValue(fileName,out byte[] luaScripts))
                Debug.LogError("lua scripts is not exist: " + fileName);
            return luaScripts;
        }
#if UNITY_EDITOR
        private void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
            for (int i = 0; i < luaFiles.Length; i++)
            {
                string fileName = PathUtil.GetStandardPath(luaFiles[i]);
                byte[] file = File.ReadAllBytes(fileName);
                AddLuaScript(PathUtil.GetUnityPath(fileName), file);
            }
            Manager.EventManager.Publish((int)GameEvent.StartLua);
        }
#endif
        private void LoadLuaScripts()
        {
            foreach (string luaName in luaNames)
            {
                Manager.Resources.LoadLua(luaName, (obj) =>
                {
                    AddLuaScript(luaName,(obj as TextAsset)?.bytes);
                    if (_luaScripts.Count >= luaNames.Count)
                    {
                        Manager.EventManager.Publish((int)GameEvent.StartLua);
                        luaNames.Clear();
                        luaNames = null;
                    }
                });
            }
        }
        private void AddLuaScript(string assetsName, byte[] luaScript)
        {
            
            _luaScripts[assetsName] = luaScript;
        }


        private void Update()
        {
            if (luaEnv != null)
            {
                luaEnv.Tick();//内存回收
            }
        }

        private void OnDestroy()
        {
            if (luaEnv != null)
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
        }
    }
    
}
