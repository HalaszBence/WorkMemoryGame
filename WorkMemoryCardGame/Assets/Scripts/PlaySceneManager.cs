using UnityEngine;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviour
{
    [SerializeField] private Text mainText;
    [SerializeField] private Text difficultyText;

    void Awake()
    {
        switch (API.instance.data.chosenGameMode)
        {
            case 1:
                mainText.text = "Új felszálló";
                break;
            case 2:
                mainText.text = "Rakd sorba és párba!";
                break;
            case 3:
                mainText.text = "Rendező";
                break;
            default:
                Debug.Log("No such case as given");
                break;
        }
    }
}
