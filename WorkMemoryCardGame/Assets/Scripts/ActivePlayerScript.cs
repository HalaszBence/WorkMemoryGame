using UnityEngine;
using UnityEngine.UI;


//This class contains the function for the active player buttons in the configuration settings, this allows only one player to be active at a time
public class ActivePlayerScript : MonoBehaviour
{
    [SerializeField] private Toggle toggle1;
    [SerializeField] private Toggle toggle2;
    [SerializeField] private Toggle activePlayer;
    [SerializeField] private InputField inputField;

    void Update()
    {
        if((toggle1.isOn || toggle2.isOn) && inputField.text != "")
        {
            activePlayer.interactable = true;
        }
        else
        {
            activePlayer.interactable = false;
            activePlayer.isOn = false;
        }
    }
}
