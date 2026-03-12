using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class SingletonClass<T> : MonoBehaviour where T : SingletonClass<T>
    {
        public static bool IsExist
        {
            get; private set;
        }

        public static T Instance
        {
            get; private set;
        }

        private static System.Action msOnInitialized;


        protected virtual void Awake()
        {
            Debug.Assert(Instance == null, $"Duplicated Singleton Class Detected, Instance Aready Enrolled [{Instance?.gameObject.name}] but you try to add [{gameObject.name}]");
            Instance = (T)this;
            IsExist = true;
            msOnInitialized?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
            IsExist = false;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            IsExist = false;
        }

        public static void AddCallbackOnInstanceInitialized(System.Action action)
        {
            if (IsExist)
            {
                action.Invoke();
                return;
            }

            msOnInitialized += action;
        }
    }
