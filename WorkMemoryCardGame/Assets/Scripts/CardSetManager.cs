using System.Collections.Generic;
using UnityEngine;


//This class contains the list of cardSets
[System.Serializable]
public class CardSetManager : MonoBehaviour
{
    #region Variables
    [SerializeField] public List<CardSet> cardSets;
    public static CardSetManager cardSetManager;
    #endregion

    void Awake()
    {
        if (cardSetManager == null)
        {
            DontDestroyOnLoad(gameObject);
            cardSetManager = this;
        }
        else if (cardSetManager != this)
        {
            Destroy(gameObject);
        }
    }

    public void addCardSet(CardSet cardset)
    {
        cardSets.Add(cardset);
    }

    public string drawAsset()
    {
        List<CardSet> possibleSets = new List<CardSet>();

        string difficulty = Profile.instance.Custom.currentDifficulty.ToString();

        switch (API.instance.data.chosenGameMode)
        {
            case 1:
                for (int i = 0; i < cardSets.Count; i++)
                {
                    if (cardSets[i].App1.Contains(difficulty)  && cardSets[i].getSize() >= 14)  //New arrival
                    {
                        possibleSets.Add(cardSets[i]);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < cardSets.Count; i++)
                {
                    if (cardSets[i].App2.Contains("1") && cardSets[i].getSize() >= 10)   //PairGame
                    {
                        possibleSets.Add(cardSets[i]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < cardSets.Count; i++)
                {
                    if (cardSets[i].App3.Contains(difficulty) && cardSets[i].getSize() >= 10)  //OrderGame
                    {
                        possibleSets.Add(cardSets[i]);
                    }
                }
                break;
            default:
                Debug.Log("No such case as given");
                break;
        }

        var rand = new System.Random();
        int number = rand.Next(possibleSets.Count);
        return possibleSets[number].getCardSetName();
    }
}
