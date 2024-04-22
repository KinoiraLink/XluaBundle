using System.Collections.Generic;
using UnityEngine;
using XLuaFramework.Behaviour;

namespace XLuaFramework.Managers
{
    public class EntityManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> _entities;
        private Dictionary<string, Transform> _groups;
        private Transform _entityParent;

        private void Awake()
        {
            _entities = new Dictionary<string, GameObject>();
            _groups = new Dictionary<string, Transform>();
            _entityParent = this.transform.parent.Find("Entity");
        }

        public void SetEntityGroup(List<string> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                GameObject group = new GameObject("Group-" + groups[i]);
                group.transform.SetParent(_entityParent, false);
                _groups[groups[i]] = group.transform;
            }
        }

        private Transform GetGroup(string group)
        {
            if (!_groups.ContainsKey(group)) Debug.LogError("group is not Exist");
            return _groups[group];
        }

        public void ShowEntity(string entityName, string group, string luaName)
        {
            if (_entities.TryGetValue(entityName, out GameObject entity))
            {
                EntityLogic logic = entity.GetComponent<EntityLogic>();
                logic.OnShow();
                return;
            }

            Manager.Resources.LoadPrefab(entityName, obj =>
            {
                entity = Instantiate(obj) as GameObject;
                Transform parent = GetGroup(group);
                if (entity != null)
                {
                    entity.transform.SetParent(parent, false);
                    _entities.Add(entityName, entity);
                    EntityLogic entityLogic = entity.AddComponent<EntityLogic>();
                    entityLogic.Init(luaName);
                    entityLogic.OnShow();
                }
            });
        }
    }
}