using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class contains the data of a cardSet
[System.Serializable]
public class CardSet
{
    #region Variables
    [SerializeField] private string cardSetName;
    [SerializeField] private string app1;
    [SerializeField] private string app2;
    [SerializeField] private string app3;
    [SerializeField] private int size;

    public string App1 { get => app1; set => app1 = value; }
    public string App2 { get => app2; set => app2 = value; }
    public string App3 { get => app3; set => app3 = value; }
    #endregion

    #region Getters / Setters
    public string getCardSetName()
    {
        return cardSetName;
    }


    public int getSize()
    {
        return size;
    }
    #endregion

    public void loadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
