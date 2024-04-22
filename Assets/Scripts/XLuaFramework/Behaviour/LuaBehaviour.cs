using System;
using UnityEngine;
using XLua;
using XLuaFramework.Managers;
// ReSharper disable All

namespace LuaBehaviour
{
    [LuaCallCSharp]
    public class LuaBehaviour : MonoBehaviour
    {
        private LuaEnv _luaEnv = Manager.LuaManager.luaEnv;
        protected LuaTable _scriptEnv;

        private Action _luaInit;
        private Action _luaUpdate;
        private Action _luaOnDestory;
        
        private void Awake()
        {
            //设置独立脚本环境
            _scriptEnv = _luaEnv.NewTable();
            LuaTable meta = _luaEnv.NewTable();
            meta.Set("__index", _luaEnv.Global);
            _scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            
            //注入自身游戏对象
            _scriptEnv.Set("self",this);
        }

        public virtual void Init(string luaName)
        {

            
            //执行lua脚本
            _luaEnv.DoString(Manager.LuaManager.GetLuaScript(luaName), luaName, _scriptEnv);
            
            _luaInit = _scriptEnv.Get<Action>("on_init");
            _luaUpdate = _scriptEnv.Get<Action>("update");
            _luaOnDestory = _scriptEnv.Get<Action>("ondestroy");
            _luaInit?.Invoke();
        }
        
        private void Update()
        {
            _luaUpdate?.Invoke();
        }

        protected virtual void Clear()
        {
            //释放委托

            _luaInit = null;
            _luaUpdate = null;
            _luaOnDestory = null;
            _scriptEnv?.Dispose();
            _scriptEnv = null;
        }

        private void OnDestroy()
        {
            _luaOnDestory?.Invoke();
            Clear();
        }

        private void OnApplicationQuit()
        {
            Clear();
        }
    }
    
    
}