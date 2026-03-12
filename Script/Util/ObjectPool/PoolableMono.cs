using UnityEngine;

    public abstract class PoolableMono : MonoBehaviour, IPoolable
    {
        public virtual void ActiveFromPool()
        {
            gameObject.SetActive(true);
        }

        public virtual bool CanReusable()
        {
            return !gameObject.activeInHierarchy;
        }

        public virtual bool CheckSame(Component comparer)
        {
            return gameObject.name == comparer.gameObject.name;
        }
    }
