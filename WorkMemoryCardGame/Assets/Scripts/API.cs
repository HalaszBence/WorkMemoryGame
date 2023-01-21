#define win 

using System.Collections;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class API : MonoBehaviour
{
    private const string ScoreGetURL = "https://laravel.etalonapps.hu/api/games/result/latest";
    public GameObject MainMenu;
    public CardManager cardManager;
    public CardSetManager cardSetManager;
    public DataFromAPI data;
    public DataToServer dataToServer;
    public testFORJSON jsonTEST;
    public int gameId;
    public int userId;
    public string token;
    public string path;
    public static API instance;

    public void Awake()
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

#if win
        string httpsLink = "https://laravel.etalonapps.hu/games/default/?user_id=973&game_id=4&token=0&config_url=https://laravel.etalonapps.hu/api/games/config/";
#else
        string httpsLink = Application.absoluteURL;
#endif
        string p = httpsLink.Split('?')[1];
        string user = p.Split('=')[1];
        string game = p.Split('=')[2];
        token = p.Split('=')[3].Split('&')[0];
        path = p.Split('=')[4];
        int.TryParse(game.Split('&')[0], out gameId);
        int.TryParse(user.Split('&')[0], out userId);
        data.gameID = gameId;
        data.userID = userId;
        data.token = token;
        data.config = path;
    }

    public void Start()
    {
        string pathToFiles;
        jsonTEST = new testFORJSON();
        if (data.config[data.config.Length - 1] == '/')
        {
            pathToFiles = data.config + data.gameID;
        }
        else
        {
            pathToFiles = data.config + "/" + data.gameID;
        }
        StartCoroutine(getData(pathToFiles));
    }

    public IEnumerator getData(string _path)
    {
#if win
        _path = "https://laravel.etalonapps.hu/api/games/config/4";
#endif
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(_path))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + unityWebRequest.error);
            }
            else
            {
                string json = unityWebRequest.downloadHandler.text;
                data = JsonUtility.FromJson<DataFromAPI>(json);
                data.gameID = gameId;
                data.userID = userId;
                data.token = token;
                data.config = path;
#if win
                data.chosenGameMode = 1;
#endif
                MainMenu.GetComponent<MainMenu>().setBackGround();
                StartCoroutine(getCardSetJSON(data.assets[1].path));
                StartCoroutine(getCardsJSON(data.assets[2].path));
                StartCoroutine(GetLatestResult());
            }
        }
    }

    public IEnumerator getCardsJSON(string path)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(path))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + unityWebRequest.error);
            }
            else
            {
                string json = unityWebRequest.downloadHandler.text;
                jsonTEST = JsonUtility.FromJson<testFORJSON>(json);
                for (int i = 0; i < jsonTEST.cards.Length; i++)
                    cardManager.addCard(jsonTEST.cards[i]);
            }
        }
    }

    public IEnumerator getCardSetJSON(string path)
    {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(path))
        {
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + unityWebRequest.error);
            }
            else
            {
                string json = unityWebRequest.downloadHandler.text;
                jsonTEST = JsonUtility.FromJson<testFORJSON>(json);
                for (int i = 0; i < jsonTEST.cardSet.Length; i++)
                    cardSetManager.addCardSet(jsonTEST.cardSet[i]);
            }
        }
    }

    public IEnumerator sendData()
    {
        dataToServer.SetAllCustomData(Profile.instance.Custom);
#if win
        dataToServer.gameId = 4;
        dataToServer.userId = 973;
        dataToServer.token = "BNT26hekY1iyRteXxVfRLN72S7M8t9q3";
#else
        dataToServer.gameId = data.gameID;
        dataToServer.userId = data.userID;
        dataToServer.token = data.token;
#endif

        string json = JsonUtility.ToJson(dataToServer);

        Debug.Log(json);

        var request = new UnityWebRequest("https://laravel.etalonapps.hu/api/games/result", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();
    }

    public testFORJSON getCards(string path)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        return JsonUtility.FromJson<testFORJSON>(json);
    }

    public IEnumerator GetLatestResult()
    {
#if win
        token = "BNT26hekY1iyRteXxVfRLN72S7M8t9q3";
        gameId = 4;
        userId = 973;
#endif

        UnityWebRequest www = UnityWebRequest.Post(ScoreGetURL, new Dictionary<string, string>()
        {
            {"token", token},
            {"gameId", gameId.ToString()},
            {"userId", userId.ToString()},
            {"count", "1"},
        });

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            if (data.chosenGameMode != 2)
            {
                Debug.Log("Error, mas jatekok inicializacio");
                AllCustomData allDataNoUserNotPair = new AllCustomData();
                allDataNoUserNotPair.data.Add(new CustomData("1", new Score(5, 0)));
                allDataNoUserNotPair.data.Add(new CustomData("2", new Score(5, 0)));
                allDataNoUserNotPair.data.Add(new CustomData("3", new Score(5, 0)));
                allDataNoUserNotPair.currentDifficulty = 1;
                Profile.instance.Custom = allDataNoUserNotPair;
            }
            else
            {
                Debug.Log("Error, parbarakos inicializacio");
                AllCustomData allDataNoUserPair = new AllCustomData();
                allDataNoUserPair.data.Add(new CustomData("1", new Score(0, 0)));
                allDataNoUserPair.data.Add(new CustomData("2", new Score(0, 0)));
                allDataNoUserPair.data.Add(new CustomData("3", new Score(0, 0)));
                allDataNoUserPair.currentDifficulty = 1;
                Profile.instance.Custom = allDataNoUserPair;
            }
        }
        else
        {
            var jArray = JArray.Parse(www.downloadHandler.text);

            AllCustomData allData = new AllCustomData();
            bool wasData = false;

            foreach (JObject jOBJ in jArray)
            {
                string jsonImportant = jOBJ.GetValue("custom").ToString();
                Debug.Log(jsonImportant);
                allData = JsonConvert.DeserializeObject<AllCustomData>(jsonImportant);
                wasData = true;
            }

            if (wasData && userId != -1)
            {
                Profile.instance.Custom = allData;
            }
            else if(data.chosenGameMode != 2)
            {
                Debug.Log("Mas jatekok inicializacio");
                AllCustomData allDataNoUserNotPair = new AllCustomData();
                allDataNoUserNotPair.data.Add(new CustomData("1", new Score(5,0)));
                allDataNoUserNotPair.data.Add(new CustomData("2", new Score(5,0)));
                allDataNoUserNotPair.data.Add(new CustomData("3", new Score(5,0)));
                allDataNoUserNotPair.currentDifficulty = 1;
                Profile.instance.Custom = allDataNoUserNotPair;
            } 
            else
            {
                Debug.Log("Parbarakos inicializacio");
                AllCustomData allDataNoUserPair = new AllCustomData();
                allDataNoUserPair.data.Add(new CustomData("1", new Score(0, 0)));
                allDataNoUserPair.data.Add(new CustomData("2", new Score(0, 0)));
                allDataNoUserPair.data.Add(new CustomData("3", new Score(0, 0)));
                allDataNoUserPair.currentDifficulty = 1;
                Profile.instance.Custom = allDataNoUserPair;
            }
        }
    }
}
