using UnityEngine.UI;
using XLua;

namespace XLuaFramework.Extension
{
    [LuaCallCSharp]
    public static class UnityEx 
    {
        //按钮扩展
        public static void OnClickSet(this Button button, object callback)
        {
            LuaFunction func = callback as LuaFunction;
            //清空按钮事件
            button.onClick.RemoveAllListeners();
            //重新订阅事件
            button.onClick.AddListener(() =>
            {
                func?.Call();
            });
        }

        //滑块扩展
        public static void OnValueChangedSet(this Slider slider, object callback)
        {
            LuaFunction func = callback as LuaFunction;
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener((float value) =>
            {
                func?.Call(value);
            });
        }
    }
}
