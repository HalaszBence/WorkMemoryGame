using UnityEngine;
using UnityEngine.UI;


//This class contains the card objects data
public class CardModel : MonoBehaviour
{
    #region Variables
    public Renderer rend;

    //The id of the card in its set
    [SerializeField] private int cardValue;

    //Unique id of the card
    [SerializeField] private int uniqueCardId;

    [SerializeField] private CardModel pair;

    [SerializeField] private Border border;

    [SerializeField] private StartButton startButton;

    [SerializeField] private Text winOrLost;
    #endregion

    #region Getters / Setters

    public void setUniqueCardId(int number)
    {
        uniqueCardId = number;
    }

    public int getUniqueCardId()
    {
        return uniqueCardId;
    }

    public void setBorder(Border val)
    {
        border = val;
    }

    public Border getBorder()
    {
        return border;
    }

    public void setPair(CardModel pair)
    {
        this.pair = pair;
    }

    public CardModel getPair()
    {
        return pair;
    }

    public int getCardValue()
    {
        return cardValue;
    }

    public void setCardValue(int id)
    {
        cardValue = id;
    }
    #endregion

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }

    public void OnMouseDown()
    {
        if (API.instance.data.chosenGameMode == 1 && GameController.instance.getCanBeSelected())
        {
            GameController.instance.lockCards();
            if (uniqueCardId == GameController.instance.getIdOfNewArrival())
            {
                Color c = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                rend.materials[1].color = c;
                FindObjectOfType<AudioManager>().Play("Win", "effect");
                GameController.instance.guessedRight(true);
            }
            else
            {
                Color c = new Color(1.0f, 0.5f, 0.9f, 1.0f);
                rend.materials[1].color = c;
                FindObjectOfType<AudioManager>().Play("Lose", "effect");
                GameController.instance.guessedRight(false);
            }
        }
    }
}
