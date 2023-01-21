using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Sprite[] sideCharacterSpirtes;
    [SerializeField] private Image sideCharacter;
    [SerializeField] private Text title;
    [SerializeField] private Text welcomeText;
    [SerializeField] private Text generalText;
    [SerializeField] private Button choosePlayer;
    private string newArrivalText = "Ezzel a játékkal azt tesztelheted, hogy meddig terjed és milyen pontos a memóriád. Ezt persze fejlesztheted is! ";
    private string orderGameText = "Ez a játék a munkamemóriádat teszteli. Ez azt jelenti, hogy nem csak meg kell jegyezned a dolgokat, hanem azokat utána megfelelő sorrendbe is kell raknod. ";
    private string pairGameText = "Ezzel a játékkal a memóriád két területét is fejlesztheted. A dolgok sorrendjének helyes felidézése ugyanúgy fontos, mint az összetartozó elemek helyes párosítása. ";


    public void setBackGround()
    {
        switch (API.instance.data.chosenGameMode)
        {
            case 1:
                title.text = "Új felszálló";
                welcomeText.text = newArrivalText;                                                         //Új felszálló
                sideCharacter.sprite = sideCharacterSpirtes[1];
                break;
            case 2:
                title.text = "Rakd sorba és párba!";
                welcomeText.text = pairGameText;
                sideCharacter.sprite = sideCharacterSpirtes[0];
                break;
            case 3:
                title.text = "Rendező";
                welcomeText.text = orderGameText;                                                         //Sorba rakós
                sideCharacter.sprite = sideCharacterSpirtes[2];
                break;
            default:
                Debug.LogError("Error with chosenGameMode value");
                break;
        }
        sideCharacter.gameObject.SetActive(true);
        generalText.gameObject.SetActive(true);
        choosePlayer.gameObject.SetActive(true);
    }

    public void QuiGamet()
    {
        Application.Quit();
    }

}
