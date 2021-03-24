using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject uiLevel1Score;
    public GameObject uiLevel2Score;
    public GameObject uiLevel3Score;

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void Awake()
    {

        this.uiLevel1Score.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0}", PlayerPrefs.GetInt("score-Level1"));
        this.uiLevel2Score.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0}", PlayerPrefs.GetInt("score-Level2"));
        this.uiLevel3Score.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("{0}", PlayerPrefs.GetInt("score-Level3"));
    }
}
