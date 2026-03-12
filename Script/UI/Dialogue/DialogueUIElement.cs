namespace UnityEngine.UI
{
    public class DialogueUIElement : MonoBehaviour
    {
        public DialogueCharacter Character
        {
            get
            {
                if(_Character == null)
                {
                    _Character = GetComponentInChildren<DialogueCharacter>();
                }
                return _Character;
            }
        }

        private DialogueCharacter _Character;
        private DialogueTextBox _TextBox;
    }
}