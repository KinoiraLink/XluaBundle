using UnityEngine;

namespace XLuaFramework.Managers
{
    [RequireComponent(typeof(LuaManager))]
    [RequireComponent(typeof(ResourcesManager))]
    [RequireComponent(typeof(UIManager))]
    [RequireComponent(typeof(EntityManager))]
    [RequireComponent(typeof(MySceneManager))]
    [RequireComponent(typeof(SoundManager))]
    [RequireComponent(typeof(EventManager))]
    [RequireComponent(typeof(PoolManager))]
    [RequireComponent(typeof(NetManager))]
    public class Manager : MonoBehaviour
    {
        private static ResourcesManager _resources;
        public static ResourcesManager Resources => _resources;

        private static LuaManager _luaManager;
        public static LuaManager LuaManager => _luaManager;
        
        private static UIManager _uiManager;
        public static UIManager UIManager => _uiManager;
        
        private static EntityManager _entityManager;
        public static EntityManager EntityManager => _entityManager;
        
        private static MySceneManager _mySceneManager;
        public static MySceneManager MySceneManager => _mySceneManager;
        
        private static SoundManager _soundManager;
        public static SoundManager SoundManager => _soundManager;

        private static EventManager _eventManager;
        public static EventManager EventManager => _eventManager;
        
        private static PoolManager _poolManager;
        public static PoolManager PoolManager => _poolManager;
        
        private static NetManager _netManager;
        public static NetManager NetManager => _netManager;
        private void Awake()
        {
            if (this.TryGetComponent( out _resources))
                Debug.Log("Init:ResourcesManager");
            
            if (this.TryGetComponent( out _luaManager))
                Debug.Log("Init:LuaManager");
            
            if (this.TryGetComponent( out _uiManager))
                Debug.Log("Init:UIManager");
            
            if (this.TryGetComponent( out _entityManager))
                Debug.Log("Init:EntityManager");
            
            if (this.TryGetComponent( out _mySceneManager))
                Debug.Log("Init:SceneManager");
            
            if (this.TryGetComponent( out _soundManager))
                Debug.Log("Init:SoundManager");
            
            if (this.TryGetComponent( out _eventManager))
                Debug.Log("Init:EventManager");
            
            if (this.TryGetComponent( out _poolManager))
                Debug.Log("Init:PoolManager");
            if (this.TryGetComponent( out _netManager))
                Debug.Log("Init:NetManager");
            
        }


    }
    
    
}