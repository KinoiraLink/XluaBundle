using UnityEngine;
using XLua;
using XLuaFramework;
using XLuaFramework.Enums;
using XLuaFramework.Managers;

public class GameStart : MonoBehaviour
{
    public GameMode gameMode;
    public bool openLog;

    private void Awake()
    {
        AppConst.gameMode = gameMode;
        AppConst.openLog = openLog;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Manager.EventManager.Subscribe((int)GameEvent.StartLua, StartLua);
        Manager.EventManager.Subscribe((int)GameEvent.GameInit, GameInit);

        if (AppConst.gameMode == GameMode.UpdateMode)
            gameObject.AddComponent<HotUpdate>();
        else
            Manager.EventManager.Publish((int)GameEvent.GameInit);
    }

    private void GameInit(object args)
    {
        if (AppConst.gameMode != GameMode.EditorMode)
            Manager.Resources.ParseVersionFile();
        Manager.LuaManager.Init();
    }

    private void OnApplicationQuit()
    {
        Manager.EventManager.UnSubscribe((int)GameEvent.StartLua, StartLua);
        Manager.EventManager.UnSubscribe((int)GameEvent.GameInit, GameInit);
    }

    private void StartLua(object args)
    {
        Manager.LuaManager.StartLua("main");
        var func = Manager.LuaManager.luaEnv.Global.Get<LuaFunction>("main");
        func.Call();

        Manager.PoolManager.CreateGameObjectPool("UI", 10);
        Manager.PoolManager.CreateGameObjectPool("Effect", 120);
        Manager.PoolManager.CreateGameObjectPool("Monster", 120);
        Manager.PoolManager.CreateAssetPool("AssetBundle", 10);
    }
}