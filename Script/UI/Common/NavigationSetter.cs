using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class NavigationSetter : DestoryOnlyIfAwakeMonoBehavior
    {
        private static int smSetterPriority = -1;
        [SerializeField] private Selectable sfFirstSelected;
        [SerializeField] private int sfSetterPriority;
        private Selectable[] mSelectableInChild;
        private bool mIsEnabled;
        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(TryUpdateUINav);
        }

        private void Update()
        {
            if (sfSetterPriority > smSetterPriority)
            {
                smSetterPriority = sfSetterPriority;
                TryUpdateUINav();
                mIsEnabled = true;
            }
            else if(sfSetterPriority < smSetterPriority)
            {
                if(mIsEnabled)
                {
                    disableNav();
                    mIsEnabled = false;
                }
            }
        }

        private void TryUpdateUINav()
        {
            if(sfSetterPriority < smSetterPriority)
            {
                return;
            }

            if (mSelectableInChild == null)
            {
                mSelectableInChild = GetComponentsInChildren<Button>();
            }

            if (InputManager.Instance.LoadedInputHandler.CurrentInputDevice == eInputDeviceType.GamePad)
            {
                sfFirstSelected.Select();
                for (int i = 0; i < mSelectableInChild.Length; i++)
                {
                    mSelectableInChild[i].navigation = Navigation.defaultNavigation;
                }
            }
            else
            {
                disableNav();
            }
        }

        private void disableNav()
        {
            Navigation noneNav = new Navigation();
            noneNav.mode = Navigation.Mode.None;
            for (int i = 0; i < mSelectableInChild.Length; i++)
            {
                mSelectableInChild[i].navigation = noneNav;
            }
        }

        private void OnEnable()
        {
            if (sfSetterPriority > smSetterPriority)
            {
                smSetterPriority = sfSetterPriority;
                TryUpdateUINav();
                mIsEnabled = true;
            }
        }

        protected override void onDisable()
        {
            if (EventSystem.current != null) //In Case EventSystem is destoryd first when scene is unloaded
            {
                EventSystem.current.firstSelectedGameObject = null;
            }
            smSetterPriority = -1;
        }

        protected override void onDestory()
        {
            if (!InputManager.IsExist)
            {
                return;
            }

            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(TryUpdateUINav);
        }
    }
}