using UnityEngine;
using TMPro;

namespace UnityEngine.UI
{
    public class SimpleTextSetter : MonoBehaviour
    {
        public void SetText(string text)
        {
            GetComponent<TMP_Text>().text = text;
        }

        public void SetText(float text)
        {
            SetText(text.ToString());
        }

        public void SetText(int text)
        {
            SetText(text.ToString());
        }

        public void SetText(Object text)
        {
            SetText(text.ToString());
        }
    }

}

