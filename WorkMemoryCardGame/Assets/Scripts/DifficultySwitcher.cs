using UnityEngine;
using UnityEngine.UI;

public class DifficultySwitcher : MonoBehaviour
{
    [SerializeField] GameObject instruction;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text question;
    [SerializeField] Text difficultyText; 
    [SerializeField] Image easyLevel; //1
    [SerializeField] Image mediumLevel; //2
    [SerializeField] Image hardLevel; //3
    private float maxScale = 1.0f;
    private float minScale = 0.6f;
    private int modeToSwitchTo = 1;
    public static DifficultySwitcher instance;

    private void Start()
    {
        instance = this;
        switch (Profile.instance.Custom.currentDifficulty)
        {
            case 1:
                difficultyText.text = "Alap";
                easyLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                mediumLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                hardLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                break;
            case 2:
                difficultyText.text = "Haladó";
                easyLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                mediumLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                hardLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                break;
            case 3:
                difficultyText.text = "Profi";
                easyLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                mediumLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                hardLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                break;
            default:
                difficultyText.text = "Alap";
                easyLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
                mediumLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                hardLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
                Debug.Log("No option like that!");
                break;
        }
    }

    public void ChangeLevelPressed(int x)
    {
        if(Profile.instance.Custom.currentDifficulty != x && instruction.activeSelf != true)
        {
            modeToSwitchTo = x;
            Time.timeScale = 0.0f;
            dialogueBox.SetActive(true);
            if(x == 1)
            {
                question.text = "Valóban alap szintre akarsz lépni?";
            }
            else if(x == 2)
            {
                question.text = "Valóban haladó szintre akarsz lépni?";
            }
            else
            {
                question.text = "Valóban profi szintre akarsz lépni?";
            }
        }
    }

    private void ReScaleIndicators(int x)
    {
        if (x == 1)
        {
            easyLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            mediumLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            hardLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            difficultyText.text = "Alap";
            Profile.instance.Custom.currentDifficulty = 1;
        }
        else if (x == 2)
        {
            easyLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            mediumLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            hardLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            difficultyText.text = "Haladó";
            Profile.instance.Custom.currentDifficulty = 2;
        }
        else
        {
            easyLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            mediumLevel.transform.localScale = new Vector3(minScale, minScale, minScale);
            hardLevel.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            difficultyText.text = "Profi";
            Profile.instance.Custom.currentDifficulty = 3;
        }

        GameController.instance.GameLevel = Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty - 1].score.score1;

        GameController.instance.StopAllCoroutines();
        GameController.instance.resetGame();
    }

    public void YesButtonPressed()
    {
        Time.timeScale = 1.0f;
        dialogueBox.SetActive(false);
        ReScaleIndicators(modeToSwitchTo);
    }

    public void NoButtonPressed()
    {
        Time.timeScale = 1.0f;
        dialogueBox.SetActive(false);
    }
}
