using UnityEngine;
using UnityEngine.UI;

public class InformationHandler : MonoBehaviour
{
    public Text helpInformation;
    public Text soundInformation;

    public void ShowHelpInformation()
    {
        helpInformation.gameObject.SetActive(true);
    }

    public void ShowSoundInformation()
    {
        soundInformation.gameObject.SetActive(true);
    }

    public void HideHelpInformation()
    {
        helpInformation.gameObject.SetActive(false);
    }

    public void HideSoundInformation()
    {
        soundInformation.gameObject.SetActive(false);
    }
}
