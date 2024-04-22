using System;

namespace XLuaFramework.Behaviour
{
    public class SceneLogic : LuaBehaviour.LuaBehaviour
    {
        public string sceneName;

        private Action _luaActive;
        private Action _luaInActive;
        private Action _luaOnEnter;
        private Action _luaOnQuit;

        public override void Init(string luaName)
        {
            base.Init(luaName);
            _scriptEnv.Get("onactive",out _luaActive);//激活
            _scriptEnv.Get("oninactive",out _luaInActive);//关闭
            _scriptEnv.Get("onenter",out _luaOnEnter);//第一次
            _scriptEnv.Get("onquit", out _luaOnQuit);//卸载
        }

        public void OnActive()
        {
            _luaActive?.Invoke();
        }

        public void InActive()
        {
            _luaInActive?.Invoke();
        }

        public void OnEnter()
        {
            _luaOnEnter?.Invoke();
        }

        public void OnQuit()
        {
            _luaOnQuit?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            _luaActive = null;
            _luaInActive = null;
            _luaOnEnter = null;
            _luaOnQuit = null;
        }
    }
}