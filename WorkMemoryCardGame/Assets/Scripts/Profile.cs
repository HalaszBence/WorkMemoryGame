using System.Collections.Generic;
using UnityEngine;
public class Profile : MonoBehaviour
{
    #region Variables
    [SerializeField] private AllCustomData custom;

    public static Profile instance;

    public AllCustomData Custom { get => custom; set => custom = value; }

    public void SetDataScore(Score _score)
    {
        custom.data[custom.currentDifficulty-1].Score = _score;
    }

    #endregion

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    //This functions loads in the data from a json file into the variables
    public void loadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}


[System.Serializable]
public struct Score
{
    public int score1;
    public int score2;

    public Score(int _score1, int _score2)
    {
        score1 = _score1;
        score2 = _score2;
    }

    public int Score1 { get => score1; set => score1 = value; }
    public int Score2 { get => score2; set => score2 = value; }
}

[System.Serializable]
public class CustomData
{
    public string difficulty;
    public Score score;

    public CustomData(string _difficulty, Score _score)
    {
        difficulty = _difficulty;
        score = _score;
    }

    public string Difficulty { get => difficulty; set => difficulty = value; }
    public Score Score { get => score; set => score = value; }
}

[System.Serializable]
public class AllCustomData
{
    public List<CustomData> data;
    public int currentDifficulty;

    public AllCustomData()
    {
        data = new List<CustomData>();
    }
}

