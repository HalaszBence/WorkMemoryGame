[System.Serializable]
public class DataFromAPI
{
    public int gameID;
    public int userID;
    public string token;
    public string config;
    public Assets[] assets;
    public int chosenGameMode;
}

[System.Serializable]
public class Assets
{
    public string name;
    public string path;
}
