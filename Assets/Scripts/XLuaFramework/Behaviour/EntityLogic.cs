using System;

namespace XLuaFramework.Behaviour
{
    public class EntityLogic : LuaBehaviour.LuaBehaviour
    {
        private Action _luaOnShow;
        private Action _luaOnHide;

        public override void Init(string luaName) //对应 Awake()
        {
            base.Init(luaName);
            _scriptEnv.Get("onshow", out _luaOnShow);
            _scriptEnv.Get("onhide", out _luaOnHide);
        }

        public void OnShow() //对应Start()
        {
            _luaOnShow?.Invoke();
        }

        public void OnHide()
        {
            _luaOnHide?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            _luaOnShow = null;
            _luaOnHide = null;
        }
    }
}