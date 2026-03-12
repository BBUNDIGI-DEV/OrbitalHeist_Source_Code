using UnityEngine;
using System.Collections.Generic;

    public class GameObjectPool : MonoBehaviour
    {
        private List<IPoolable> mObjectList = new List<IPoolable>();

        public static GameObjectPool TryGetGameobjectPool(Transform parent, string name)
        {
            Transform transform = parent.FindRecursive(name);

            Debug.Assert(transform != null, $"Cannot found Gameobject [{name}] from [{parent.name}]");
            GameObjectPool pool = transform.GetComponent<GameObjectPool>();
            if (pool == null)
            {
                pool = transform.gameObject.AddComponent<GameObjectPool>();
            }
            return pool;
        }

        public static GameObjectPool TryGetGameobjectPoolByTag(string name)
        {
            GameObject taggedGameobject = GameObject.FindGameObjectWithTag(name);
            Debug.Assert(taggedGameobject != null, $"Cannot found Gameobject [{name}] from [{taggedGameobject.name}]");
            GameObjectPool pool = taggedGameobject.GetComponent<GameObjectPool>();
            if (pool == null)
            {
                pool = taggedGameobject.gameObject.AddComponent<GameObjectPool>();
            }
            return pool;
        }

        public T GetGameobject<T>(T prefab) where T : Component, IPoolable
        {
            for (int i = 0; i < mObjectList.Count; i++)
            {
                IPoolable checkObject = mObjectList[i];
                if (checkObject.CanReusable() && checkObject.CheckSame(prefab) && checkObject is T)
                {
                    checkObject.ActiveFromPool();
                    return checkObject as T;
                }
            }

            T clone = instantiateGameobject(prefab);
            clone.gameObject.SetActive(true);
            mObjectList.Add(clone);
            clone.ActiveFromPool();
            return clone;
        }

        public bool TryCheckActivatedObjectExist<T>(T prefab) where T : PoolableMono
        {
            for (int i = 0; i < mObjectList.Count; i++)
            {
                IPoolable checkObject = mObjectList[i];
                if(checkObject.CheckSame(prefab) && checkObject is T)
                {
                    if(!checkObject.CanReusable())
                    {
                        return true;
                    }
                }    
            }

            return false;
        }

        private T instantiateGameobject<T>(T prefab) where T : Component, IPoolable
        {
            T clone = Instantiate(prefab, transform);
            clone.name = prefab.name;
            return clone;
        }
    }
