using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [SerializeField] private GameObject helpInformation;

    public void OnMouseDown()
    {

        switch (API.instance.data.chosenGameMode)
        {
            case 1:
                helpInformation.GetComponentInChildren<Text>().text = Constants.newArrivalRule;

                break;
            case 2:
                helpInformation.GetComponentInChildren<Text>().text = Constants.pairGameRule;

                break;
            case 3:
                helpInformation.GetComponentInChildren<Text>().text = Constants.orderGameRule;
                break;
            default:
                Debug.Log("No such case as given");
                break;
        }

        helpInformation.SetActive(true);
        if(!GameController.instance.firstGameOfTheDay)
            Time.timeScale = 0.0f;
    }

    public void OnMouseEnter()
    {
        GameObject.FindGameObjectWithTag("InformationHandler").GetComponent<InformationHandler>().ShowHelpInformation();
    }

    public void OnMouseExit()
    {
        GameObject.FindGameObjectWithTag("InformationHandler").GetComponent<InformationHandler>().HideHelpInformation();
    }

    public void HideInformation()
    {
        helpInformation.SetActive(false);
        Time.timeScale = 1.0f;
        GameController.instance.firstGameOfTheDay = false;
    }
}

