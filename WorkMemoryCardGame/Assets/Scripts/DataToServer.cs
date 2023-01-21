using UnityEngine;

[System.Serializable]
public class DataToServer 
{
    public string token;
    public int gameId;
    public int userId;
    public AllCustomData custom;

    public void SetAllCustomData(AllCustomData _data)
    {
        custom = _data;      
    }
}
