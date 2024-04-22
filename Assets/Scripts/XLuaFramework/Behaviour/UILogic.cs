using System;
using XLuaFramework.Managers;

// ReSharper disable All

namespace LuaBehaviour
{
    public class UILogic : LuaBehaviour
    {
        public string AssetName { get; set; }

        private Action m_LuaOnOpen;
        private Action m_LuaOnClose;

        public override void Init(string luaName)
        {
            base.Init(luaName);
            m_LuaOnOpen = _scriptEnv.Get<Action>("OnOpen");
            m_LuaOnClose = _scriptEnv.Get<Action>("OnClose");
        }
        
        public void OnOpen()
        {
            m_LuaOnOpen?.Invoke();
        }

        public void OnClose()
        {
            m_LuaOnClose?.Invoke();
            Manager.PoolManager.UnSpawn("UI",AssetName,this.gameObject);
        }

        protected override void Clear()
        {
            base.Clear();
            m_LuaOnOpen = null;
            m_LuaOnClose = null;
        }
        
        
    }
}