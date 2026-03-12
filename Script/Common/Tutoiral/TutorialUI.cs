using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject[] sfText;

    public void SetTutorialUI(TutorialManager.eTutorialState state)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < sfText.Length; i++)
        {
            sfText[i].SetActive(false);
        }

        sfText[(int)state].SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct TutorialUIData
{
    public string Contents;
}
