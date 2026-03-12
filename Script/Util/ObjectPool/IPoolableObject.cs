using UnityEngine;

    public interface IPoolable
    {
        public bool CanReusable();
        public bool CheckSame(Component comparer);
        public void ActiveFromPool();
    }
