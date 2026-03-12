using TMPro;
using Febucci.UI.Core;
using System.Collections;

namespace UnityEngine.UI
{
    public class CutSceneInitialCutHandler : MonoBehaviour
    {
        private const string SHOW_FIRST_CUT = "ANI_show_first_cut";
        private const string START_CUT_SCENE = "ANI_start_story_cutscene";
        [SerializeField] private string sfInitialText;
        [SerializeField] private TMP_Text sfText;
        private void Awake()
        {
            sfText.gameObject.SetActive(false);
        }

        public IEnumerator Start()
        {
            yield return new WaitForSeconds(1.0f);
            sfText.gameObject.SetActive(true);
            sfText.text = sfInitialText;
            sfText.GetComponent<TypewriterCore>().ShowText(sfInitialText);
            GetComponent<Animation>().Play(START_CUT_SCENE);
        }

        public void ShowFirstScene()
        {
            GetComponent<Animation>().Play(SHOW_FIRST_CUT);
        }

    }
}