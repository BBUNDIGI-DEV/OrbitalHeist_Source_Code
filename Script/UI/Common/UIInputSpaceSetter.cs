namespace UnityEngine.UI
{
    public class UIInputSpaceSetter : MonoBehaviour
    {
        private eInputSections mPrevSection;

        private void OnEnable()
        {
            mPrevSection = InputManager.Instance.CurrentInputSection;
            InputManager.Instance.SwitchInputSection(eInputSections.UI);
        }

        private void OnDisable()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.SwitchInputSection(mPrevSection);
            }
        }
    }
}