using UnityEngine;


//This class contains the data of a card
[System.Serializable]
public class Card
{
    #region Variables
    [SerializeField] private string cardSet;

    [SerializeField] private string cardName;

    //The id of the card but not in its own set but all the cards
    [SerializeField] private int uniqueId;

    //The value of the card in its set
    [SerializeField] private int cardValue;
    #endregion


    //Loads in data form a json file
    public void loadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }


    #region Getters / Setters
    //Getters and setters for the variables
    public string getCardName()
    {
        return cardName;
    }

    public int getUniqueId()
    {
        return uniqueId;
    }

    public int getCardValue()
    {
        return cardValue;
    }

    public string getCardSet()
    {
        return cardSet;
    }
    #endregion
}
