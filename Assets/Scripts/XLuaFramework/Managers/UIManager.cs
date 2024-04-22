using System.Collections.Generic;
using LuaBehaviour;
using UnityEngine;
using XLuaFramework.Util;
using Object = UnityEngine.Object;

namespace XLuaFramework.Managers
{
    public class UIManager : MonoBehaviour
    {
        //缓存 UI //后期更换为对象池
        //private Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

        //UI 分组
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, Transform> _UIGroups;

        // ReSharper disable once InconsistentNaming
        private Transform _UIParent;
        private void Awake()
        {   
            _UIGroups = new Dictionary<string, Transform>();
            _UIParent = this.transform.parent.Find("UI");
        }

        public void SetUIGroup(List<string> group)
        {
            for (int i = 0; i < group.Count; i++)
            {
                GameObject go = new GameObject("Group-" + group[i]);
                go.transform.SetParent(_UIParent,false);
                _UIGroups.Add(group[i],go.transform);
            }
        }

        private Transform GetUIGroup(string group)
        {
            if (!_UIGroups.ContainsKey(group))
            {
                Debug.Log("group is not exist");
            }

            return _UIGroups[group];
        }

        public void OpenUI(string uiName,string group, string luaName)
        {
            GameObject ui;
            Transform parent = GetUIGroup(group);
            string uiPath = PathUtil.GetUIPath(uiName);
            Object uiObj = Manager.PoolManager.Spawn("UI", uiPath);
            if (uiObj != null)
            {
                ui = uiObj as GameObject;
                if (ui != null)
                {
                    ui.transform.SetParent(parent, false);
                    UILogic uiLogic = ui.GetComponent<UILogic>();
                    uiLogic.OnOpen();
                }

                return;
            }
            Manager.Resources.LoadUI(uiName, obj =>
            {
                ui = Instantiate(obj) as GameObject;
                //m_UI.Add(uiName,ui);


                if (ui != null)
                {
                    ui.transform.SetParent(parent, false);
                    UILogic uiLogic = ui.AddComponent<UILogic>();
                    uiLogic.AssetName = uiPath;
                    uiLogic.Init(luaName);
                    uiLogic.OnOpen();
                }
            });
        }

        public void PrintUI(string a)
        {
            Debug.Log(a);
        }
    }
}