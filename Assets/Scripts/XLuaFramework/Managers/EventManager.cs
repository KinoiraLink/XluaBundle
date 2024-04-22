using System.Collections.Generic;
using UnityEngine;

namespace XLuaFramework.Managers
{
    public class EventManager : MonoBehaviour
    {
        public delegate void EnvHandler(object args);

        private Dictionary<int, EnvHandler> _events;

        private void Awake()
        {
            _events = new Dictionary<int, EnvHandler>();
        }

        public void Subscribe(int id, EnvHandler e)
        {
            if (_events.ContainsKey(id))
            {
                _events[id] += e;
            }
            else
            {
                _events.Add(id,e);
            }
        }

        public void UnSubscribe(int id, EnvHandler e)
        {
            if (_events.ContainsKey(id))
            {
                if (_events[id] != null)
                    _events[id] -= e;
                if (_events[id] == null)
                    _events.Remove(id);
            }
        }

        public void Publish(int id, object args = null)
        {
 
            if (_events.TryGetValue(id, out EnvHandler handler))
            {
                handler(args);
            }
        }
    }
}