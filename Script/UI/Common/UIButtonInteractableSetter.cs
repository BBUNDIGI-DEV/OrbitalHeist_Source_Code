namespace UnityEngine.UI
{
    public class UIButtonInteractableSetter : MonoBehaviour
    {
        public void EnableAllButton()
        {
            Button[] buttons = GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = true;
            }
        }

        public void DisableAllButton()
        {
            Button[] buttons = GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
        }
    }
}