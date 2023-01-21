//#define TESTMODE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

static class Constants
{
    #region Constant variables
    //Timing:
#if TESTMODE
    public const float speedOfFirstPositioning = 0.5f;
    public const float speedOfZeroing = 0.3f; //The time in the NewArrival game when all the cards go to the (0,0,0) position, that is the starting position of the shuffle phase
    public const float speedOfArrivalZeroing = 0.5f; //The time when the new arrival card is created and moved to the (0,0,0) position, the next phase is the shuffle phase
    public const float speedOfThreeShuffles = 0.1f; //The time of shuffling the deck after the new arrival is in position
    public const float waitTimeInPairGame = 1.0f; //The time we wait after one pair is in focus in the pairing game. They arrive at the give position then wait this much time and move out of the screen
    public const float speedOfPairGame = 0.5f; //The time it takes for the cards to arrive at the given destination where they stay for some time to be examined by the players
    public const float shuffleDelay = 1.0f;  //The time between two shuffles in the new arrival game
    public const float cardChangeSpeed = 0.2f;  //The time it takes for a card to move to its original place if it can't be placed where the player wanted to
#else
    public const float speedOfFirstPositioning = 0.5f;
    public const float speedOfZeroing = 0.3f; //The time in the NewArrival game when all the cards go to the (0,0,0) position, that is the starting position of the shuffle phase
    public const float speedOfArrivalZeroing = 0.5f; //The time when the new arrival card is created and moved to the (0,0,0) position, the next phase is the shuffle phase
    public const float speedOfThreeShuffles = 0.1f; //The time of shuffling the deck after the new arrival is in position
    public const float waitTimeInPairGame = 3.5f; //The time we wait after one pair is in focus in the pairing game. They arrive at the give position then wait this much time and move out of the screen
    public const float speedOfPairGame = 2.0f; //The time it takes for the cards to arrive at the given destination where they stay for some time to be examined by the players
    public const float shuffleDelay = 1.0f;  //The time between two shuffles in the new arrival game
    public const float cardChangeSpeed = 0.2f;  //The time it takes for a card to move to its original place if it can't be placed where the player wanted to
#endif

    public const string newArrivalRule = "A lefordított kártyalapokat egyszer felfordítjuk, amikor megfigyelheted őket. Ezután újraosztjuk a kártyákat, amibe belekeverünk egy olyan lapot, ami az előző leosztásban nem szerepelt. Ha megfordulnak a kártyák, kattints arra a lapra, amelyik az „új felszálló”!\nKét helyes válasz után eggyel növeljük, két helytelen válasz után eggyel csökkentjük a kiosztott kártyalapok számát.";
    public const string orderGameRule = "A lefordított kártyalapokat egyszer felfordítjuk, amikor megfigyelheted őket. Ezután emlékezetből balról jobbra haladva rakd növekvő sorrendbe vagy betűrendbe a kártyákat!\nAz azonos értékű lapok egymás mellé kerülnek, és a sorrendjük nem számít.\nKét helyes válasz után eggyel növeljük, két helytelen válasz után eggyel csökkentjük a kiosztott kártyalapok számát.";
    public const string pairGameRule = "Most 10 pár kártyát fogsz látni. Meg kell jegyezned a bal oldali kártyák egymás utáni sorrendjét, és azt, hogy ezeknek mi a párja! A 10 pár ábrából álló sorozatot háromszor nézheted meg.\nEzután először emlékezetből sorba kell állítanod a bal oldalon megjelent kártyákat, majd alájuk kell raknod a párjukat.";

    public const string newArrivalTaskHelp1 = "Figyeld meg a lapokat!";
    public const string newArrivalTaskHelp2 = "";
    public const string newArrivalTaskHelp3 = "Válaszd ki az új lapot!";

    public const string pairTaskHelp1 = "Nézd meg a sorozatot először!";
    public const string pairTaskHelp2 = "Nézd meg a sorozatot másodszor!";
    public const string pairTaskHelp3 = "Nézd meg a sorozatot harmadszor!";
    public const string pairTaskHelp4 = "Rakd sorba a kártyák!";
    public const string pairTaskHelp5 = "Húzd a kártyák alá a párjukat!";

    public const string orderTaskHelp1 = "Figyeld meg a lapokat!";
    public const string orderTaskHelp2 = "Rakd sorba a kártyákat!";

    //Positioning:
    public const float cardSize = 1.812f;
    public const float padding = 0.2f;

    //The three Y positions for the new arrival game. yMid is when there is only one line of cards, yUp and yDown when the cards are in two lines
    public const float yMid = -0.5f; 
    public const float yUp = 1.0f;
    public const float yDown = -2.0f;

    public const float yOrder = -2.0f;  //Y position of the cards in the order game
    public const float yOrderBorder = 1.0f; //Y position for the borders in the order game



    public const float yPairUpper = 1.0f;  //Y value of the upper row of cards (In the last phase there are 3 different rows of cards)
    public const float yPairLower = -4.8f; //Y value for the lowest row of cards (In the last phase there are 3 different rows of cards)
    public const float yPairMid = -1.9f; //Y value for the middle row of cards (In the last phase there are 3 different rows of cards)
    public const float yPairInit = -0.4f;  //Y position for the cards where we show the players the different pairs.
    public const float yPairTemp = -10.0f;  //Y value for a temporary position, the only purpose is that the cards are not visible at that y position

    //Game settings:
    public const int minLevelOfNewArrival = 4;
    public const int maxLevelOfNewArrival = 14;
    public const int minLevelOfOrderGame = 4;
    public const int maxLevelOfOrderGame = 9;
#endregion
}


public class GameController : MonoBehaviour
{
    //Info for myself:
    // New Arrival : 1
    // Pair Game : 2
    // Order Game : 3

    #region Variables
    [SerializeField] private Button orderGame;

    //Game:
    private int idOfNewArrival = 0; //Contains the id of the new arrival
    private int gameLevel; //Game level
    private GameObject[] objs; //Container for the cards
    [SerializeField] private ArrayList listForMain = new ArrayList();
    [SerializeField] private bool canBeSelected = false; //Engedélyező, hogy megtippelhessük az új felszállót
    private int rightGuesses = 0; //Counter a jó tippekhez
    private int wrongGuesses = 0; //Counter a rossz tippekhez
    private int counterForPairGame = 0;
    private GameObject cardCollectionManager;
    private GameObject cardSetCollectionManager;
    private CardManager cardManager;
    private CardSetManager cardSetManager;
    public ArrayList normalIds = new ArrayList();
    ArrayList orderedIds = new ArrayList();
    public ArrayList ids1 = new ArrayList();
    public ArrayList ids2 = new ArrayList();
    public string assetName1;
    public string assetName2;
    [SerializeField] private Text mainText;
    public bool firstRun = true;
    private List<GameObject> orderOfCardsInPairGame = new List<GameObject>();
    public GameObject wonMessage;
    public GameObject lostMessage;
    public GameObject orderPairGameEndDialogue;
    public DrawNewAssetButton newAssetDrawer;
    public Text orderResult;
    public Text pairResult;
    public TextMeshProUGUI orderGameCheat;
    public Text taskMessage;
    public Button imReadyButtonOrder;
    public Button imReadyButtonPair1;
    public Button imReadyButtonPair2;
    public bool pairGameSecondCollectionOfCardsArePlayable = false;
    public bool firstGameOfTheDay = true;
    [SerializeField] private GameObject pairGameOrderIndicator;
    [SerializeField] GameObject difficultySwitcher;

    //Timing:
    private float waitTimeForRevealReference = 2;
    private float waitTimeForReveal = 2; //Mennyi idő teljen el aközött, hogy kitettük a lapokat és elkezdjük felfordítani őket.
    private float waitBetweenCardsAndNewArrival = 2;  //Mennyi idő teljen el az összes lap felfordítása után ameddig az új felszállót betesszük
    private float waitBetweenCardsFlipping = 2; //Mennyi idő teljen el az egyik lap lefordítása és a másik lap felfordítása között

    //Assets:
    public List<Texture2D> textures1; //A textúrákat itt tároljuk el, amiket betöltünk
    public List<Texture2D> textures2; //A textúrákat itt tároljuk el, amiket betöltünk
    public LoadAsset loadAsset; //Hivatkozás a LoadAsset osztályra


    //Camera settings:
    new private GameObject camera;
    private readonly float[] cameraZPosNewArrival = { -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.0f, -8.5f, -9.0f, -9.0f, -10.5f };
    private readonly float[] cameraZPosOrder = { -8.0f, -8.0f, -8.0f, -9.0f, -9.0f, -9.0f, -10.0f, -12.0f, -12.0f, -10.0f, -10.0f, -12.0f, -12.0f, -12.0f };
    private readonly float[] cameraZPosPair = { -13.5f, -13.5f, -13.5f, -13.5f, -13.5f, -13.5f, -13.5f, -13.5f, -13.5f, -13.0f, -13.5f, -13.5f, -13.5f, -13.5f };
    private readonly float cameraYPosPairGame = -1.4f;
    #endregion

    #region instance
    public static GameController instance;

    public int GameLevel { get => gameLevel; set => gameLevel = value; }
    #endregion


    void Awake()
    {
        instance = this;

        if (API.instance.userId == -1)
        {
            gameLevel = 5;
        }
        else if (API.instance.gameId != 2)
        {
            gameLevel = Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty - 1].score.score1;
        }

        cardSetCollectionManager = GameObject.FindGameObjectWithTag("cardSetCollectionManager");
        cardCollectionManager = GameObject.FindGameObjectWithTag("cardCollectionManager");
        cardManager = cardCollectionManager.GetComponent<CardManager>();
        cardSetManager = cardSetCollectionManager.GetComponent<CardSetManager>();
        camera = GameObject.Find("CardCamera");
        
        assetName1 = cardSetManager.drawAsset();

        if (API.instance.data.chosenGameMode == 2)
        {
            int k = 0;
            while (k != 1)
            {
                assetName2 = cardSetManager.drawAsset();
                if (assetName2 != assetName1)
                {
                    k++;
                }
            }
            loadAsset.loadAssetBundles(assetName1, assetName2);
        }
        else
        {
            loadAsset.loadAssetBundle(assetName1);
        }

        GameObject.FindGameObjectWithTag("HelpButton").GetComponent<HelpButton>().OnMouseDown();
    }


    #region OrderGameCode
    public void putThemInOrder()
    {
        difficultySwitcher.SetActive(true);

        for (int i = 0; i < cardManager.containerOfCards1.Count; i++)
        {
            ids1.Add(cardManager.containerOfCards1[i].getCardValue());
        }

        for (int i = 0; i < cardManager.containerOfCards1.Count; i++)
        {
            orderedIds.Add(cardManager.containerOfCards1[i].getCardValue());          //Itt nyúltam bele
        }

        camera.GetComponent<CameraMovement>().setMovement(0, -1.2f, cameraZPosOrder[gameLevel - 1], true, 0.5f);

        GameObject[] parent = GameObject.FindGameObjectsWithTag("instantiateParent");
        GameObject[] border = GameObject.FindGameObjectsWithTag("instantiateBorder");

        for (int i = 0; i < gameLevel; i++)
        {
            var card = Instantiate(parent[0], new Vector3(0, 0, 0), Quaternion.identity);
            var bord = Instantiate(border[0], new Vector3(0, 10, 0), Quaternion.identity);
            var starterBord = Instantiate(border[0], new Vector3(0, 10, 0), Quaternion.identity);

            card.tag = "Parent";
            card.GetComponentInChildren<CardModel>().tag = "CardModel";
            card.GetComponentInChildren<CardModel>().setBorder(starterBord.GetComponent<Border>());

            bord.tag = "Border";
            bord.GetComponent<Border>().setIsAvailable(true);


            starterBord.tag = "starterBorder";
            starterBord.GetComponent<Border>().setIsStarter(true);
            starterBord.GetComponent<Border>().setIsAvailable(true);
            starterBord.GetComponent<Border>().setOccupied(true);
            starterBord.GetComponent<Border>().setCard(card.GetComponentInChildren<CardModel>());
        }

        loadInObjects();
        for (int i = 0; i < listForMain.Count; i++)
        {
            ((GameObject)listForMain[i]).GetComponent<CardModel>().rend.materials[1].mainTexture = textures1[i];
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setUniqueCardId(cardManager.containerOfCards1[i].getUniqueId());
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setCardValue(cardManager.containerOfCards1[i].getCardValue());
        }

        orderedIds.Sort();

        parent = GameObject.FindGameObjectsWithTag("Parent");

        border = GameObject.FindGameObjectsWithTag("Border");

        GameObject[] starterBorder = GameObject.FindGameObjectsWithTag("starterBorder");

        shuffleIndexes(ids1);

        startPositioning(parent, 0.2f, 0.2f, 0.1f);

        posCardsForOrder(parent, Constants.yOrder, 0.0f);

        posCardsForOrder(starterBorder, Constants.yOrder, 0.01f);

        for (int i = 0; i < starterBorder.Length; i++)
        {
            starterBorder[i].tag = "Border";
        }

        posCardsForOrder(border, Constants.yOrderBorder, 0.01f);



        //Kiírom hogy mi a megfelelő sorrend a lapoknál mint segítség!!!
        GameObject[] cardsList = GameObject.FindGameObjectsWithTag("CardModel");
        List<int> numbers = new List<int>();
        foreach (GameObject go in cardsList)
        {
            numbers.Add(go.GetComponent<CardModel>().getUniqueCardId());
        }

        List<int> maximumList = new List<int>();
        while (numbers.Count > 0)
        {
            int minValue = numbers.Min();
            int minIndex = numbers.ToList().IndexOf(minValue);
            maximumList.Add(minIndex);
            numbers.RemoveAt(minIndex);
        }

        for (int i = 0; i < maximumList.Count; i++)
        {
            orderGameCheat.text += maximumList[i];
        }

        StartCoroutine(HoldGame(0.5f));

        UpdateTaskMessage(Constants.orderTaskHelp1);

        //Felfordítjuk a kártyákat egyesével
        StartCoroutine(revealCards(parent));
    }
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------

    #region PairGame
    public void pairThem()
    {
        GameObject[] parent = GameObject.FindGameObjectsWithTag("instantiateParent");
        GameObject.FindGameObjectWithTag("BackGround").GetComponent<BackGroundScaler>().SetScale(4.0f);
        float x = -5;
        float y = 10;
        float z = 0;

        for (int i = 0; i < 20; i++)
        {
            var card = Instantiate(parent[0], new Vector3(x, y, z), Quaternion.identity);
            card.tag = "Parent";
            card.GetComponentInChildren<CardModel>().tag = "CardModel";
            card.transform.Rotate(0, 180, 0);

            if (i == 10 - 1)
            {
                x = 5;
            }
        }

        loadInObjects();

        for (int i = 0; i < listForMain.Count / 2; i++)
        {
            ((GameObject)listForMain[i]).GetComponent<CardModel>().rend.materials[1].mainTexture = textures1[i];
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setCardValue(cardManager.containerOfCards1[i].getCardValue());
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setUniqueCardId(cardManager.containerOfCards1[i].getUniqueId());
        }

        for (int j = listForMain.Count / 2; j < listForMain.Count; j++)
        {
            ((GameObject)listForMain[j]).GetComponent<CardModel>().rend.materials[1].mainTexture = textures2[listForMain.Count - j - 1];
            ((GameObject)listForMain[j]).GetComponent<CardModel>().setCardValue(cardManager.containerOfCards2[listForMain.Count - j - 1].getCardValue());
            ((GameObject)listForMain[j]).GetComponent<CardModel>().setUniqueCardId(cardManager.containerOfCards2[listForMain.Count - j - 1].getUniqueId());
        }

        for (int j = 0; j < 10; j++)
        {
            GameObject[] border = GameObject.FindGameObjectsWithTag("instantiateBorder");

            var bordStarter = Instantiate(border[0], new Vector3(0, 10, 0), Quaternion.identity);
            var bordPair1 = Instantiate(border[0], new Vector3(0, 10, 0), Quaternion.identity);
            var bordPair2 = Instantiate(border[0], new Vector3(0, 10, 0), Quaternion.identity);

            bordStarter.tag = "starterBorder";
            bordStarter.GetComponent<Border>().setIsAvailable(true);
            bordStarter.GetComponent<Border>().setIsStarter(true);

            bordPair1.tag = "pair1Border";
            bordPair1.GetComponent<Border>().setIsAvailable(false);

            bordPair2.tag = "pair2Border";
            bordPair2.GetComponent<Border>().setIsAvailable(true);
        }

        if (firstGameOfTheDay)
        {
            Time.timeScale = 0.0f;
            firstGameOfTheDay = false;
        }

        StartCoroutine(showPair());
    }
    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------

    #region NewArrivalGame
    public void newArrival()
    {
        difficultySwitcher.SetActive(true);

        for (int i = 0; i < cardManager.containerOfCards1.Count; i++)
        {
            ids1.Add(cardManager.containerOfCards1[i].getUniqueId());
        }

        camera.GetComponent<CameraMovement>().setMovement(0, -1.05f, cameraZPosNewArrival[gameLevel - 1], true, 0.5f);

        float delay = gameLevel * waitBetweenCardsFlipping + waitTimeForReveal + waitBetweenCardsAndNewArrival - 2;

        GameObject parentInstantiate = GameObject.FindGameObjectWithTag("instantiateParent");

        //Felrakom a kártyákat
        for (int i = 0; i < gameLevel; i++)
        {
            var card = Instantiate(parentInstantiate, new Vector3(0, 0, 0), Quaternion.identity);
            card.tag = "Parent";
            card.GetComponentInChildren<CardModel>().tag = "CardModel";
        }

        loadInObjects();


        for (int i = 0; i < listForMain.Count; i++)
        {
            ((GameObject)listForMain[i]).GetComponent<CardModel>().rend.materials[1].mainTexture = textures1[i];
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setCardValue(cardManager.containerOfCards1[i].getCardValue());
            ((GameObject)listForMain[i]).GetComponent<CardModel>().setUniqueCardId(cardManager.containerOfCards1[i].getUniqueId());
        }

        //Megkeressük az összes kártyát és megfelelően pozícionáljuk őket
        GameObject[] parent = GameObject.FindGameObjectsWithTag("Parent");

        shuffleIndexes(ids1);

        startPositioning(parent, 0.2f, 0.2f, 0.1f);

        positionCards2(parent);

        UpdateTaskMessage(Constants.newArrivalTaskHelp1);

        //Felfordítjuk a kártyákat egyesével
        for (int j = 0; j < listForMain.Count; j++)
        {
            for (int i = 0; i < ids1.Count; i++)
            {
                if (parent[i].GetComponentInChildren<CardModel>().getUniqueCardId() == (int)ids1[j])
                {
                    GameObject go = parent[i];
                    ParentScript ps = go.GetComponent<ParentScript>();
                    StartCoroutine(waitABitAndReveal(waitTimeForReveal, ps));
                    waitTimeForReveal += waitBetweenCardsFlipping;
                }
            }
        }

        StartCoroutine(HoldGame(waitTimeForRevealReference -  0.1f));

        StartCoroutine(waitAndPositionAllCardsInZero(delay + Constants.shuffleDelay));

        //Kirakjuk az új felszállót és becsúsztatjuk a 0,0-ba
        StartCoroutine(WaitaBitAndAddNewArrival(delay + Constants.shuffleDelay + 1.0f));

        //Ide jön a shuffle:
        StartCoroutine(shuffle(parent, delay + Constants.shuffleDelay + 2.0f));

        //Megkeverjük, újra kitesszük
        StartCoroutine(waitABitAndDoAll(delay + Constants.shuffleDelay + 3.5f));

        //Felfordítjuk egyesével a kártyákat és várunk a tippre
        StartCoroutine(tippingPosition(delay + Constants.shuffleDelay + 5.0f));
    }
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------

    #region OrderGameFunctions
    //Functions for game no 1. (Order game):
    private void posCardsForOrder(GameObject[] parent, float y, float z)
    {
        double x = calcx(parent.Length, 1.0f);
        for (int i = 0; i < parent.Length; i++)
        {
            parent[i].transform.position = new Vector3((float)x, y, z);
            x += Constants.padding + Constants.cardSize;
        }
    }

    IEnumerator revealCards(GameObject[] parent)
    {
        yield return new WaitForSeconds(waitTimeForReveal);

        for (int i = 0; i < ids1.Count; i++)
        {
            GameObject go = parent[i];
            ParentScript ps = go.GetComponent<ParentScript>();

            if (ps != null)
            {
                ps.revealAndHide();
                yield return new WaitForSeconds(2);
            }
        }

        for (int i = 0; i < listForMain.Count; i++)
        {
            parent[i].GetComponentInChildren<DragAndDrop>().setDrag(true);
        }

        UpdateTaskMessage(Constants.orderTaskHelp2);
    }

    public void evaluateOrder()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
        int i = 0;
        int counter = 0;
        while (i < borders.Length + 1)
        {
            if (borders[i].GetComponent<Border>().getOccupied() == true)
            {
                i++;
            }
            else
            {
                break;
            }
        }
        if (i == borders.Length / 2)
        {
            for (int j = 0; j < borders.Length / 2; j++)
            {
                if (borders[j].GetComponent<Border>().getCard().getCardValue() <= (int)orderedIds[j])      //Itt nyúltam bele!!!!
                {
                    counter++;
                }
            }

            GameObject[] cards = GameObject.FindGameObjectsWithTag("Parent");
            for (int j = 0; j < cards.Length; j++)
            {
                cards[j].GetComponent<ParentScript>().reveal();
            }

            if (counter == gameLevel)
            {
                guessedRight(true);
            }
            else
            {
                guessedRight(false);
            }
        }
        else
        {
            Debug.Log("There are missing cards");
        }
    }
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------

    #region PairGameFunctions
    //Functions for game no. 2 (Pair game):
    IEnumerator showPair()
    {
        if(counterForPairGame == 0)
            UpdateTaskMessage(Constants.pairTaskHelp1);

        camera.GetComponent<CameraMovement>().setMovement(0, -0.6459083f, -5.260495f, true, 0.5f);

        GameObject[] parent = GameObject.FindGameObjectsWithTag("Parent");

        pairGameOrderIndicator.GetComponent<Animator>().SetBool("CanOpen", true);

        for (int i = 0; i < 10; i++)
        {
            if (counterForPairGame == 0)
            {
                parent[i].GetComponentInChildren<CardModel>().setPair(parent[2 * 10 - i - 1].GetComponentInChildren<CardModel>());
                orderOfCardsInPairGame.Add(parent[i]);
            }

            positionForSmoothStep(parent[i], -1.5f, Constants.yPairInit, 0, true, Constants.speedOfPairGame);

            positionForSmoothStep(parent[2 * 10 - i - 1], 1.5f, Constants.yPairInit, 0, true, Constants.speedOfPairGame);

            yield return new WaitForSeconds(Constants.waitTimeInPairGame);

            positionForSmoothStep(parent[i], -5, -10, 0, true, Constants.speedOfPairGame);

            positionForSmoothStep(parent[2 * 10 - i - 1], 5, -10, 0, true, Constants.speedOfPairGame);
        }

        yield return new WaitForSeconds(3);

        for (int i = 0; i < 10; i++)
        {
            parent[i].transform.position = new Vector3(-5, 10, 0);
            parent[2 * 10 - i - 1].transform.position = new Vector3(5, 10, 0);
        }

        counterForPairGame += 1;
        if (counterForPairGame < 3)
        {
            if(counterForPairGame == 1)
            {
                pairGameOrderIndicator.GetComponentInChildren<Text>().text = "2";
                UpdateTaskMessage(Constants.pairTaskHelp2);
            }
            else if(counterForPairGame == 2)
            {
                pairGameOrderIndicator.GetComponentInChildren<Text>().text = "3";
                UpdateTaskMessage(Constants.pairTaskHelp3);
            }
            StartCoroutine(showPair());
        }

        if (counterForPairGame == 3)
        {
            UpdateTaskMessage(Constants.pairTaskHelp4);

            pairGameOrderIndicator.GetComponent<Animator>().SetBool("CanOpen", false);

            camera.GetComponent<CameraMovement>().setMovement(0, cameraYPosPairGame, cameraZPosPair[5], true, 1.0f);

            double x = calcx(10, 1);

            GameObject[] starterBorder = GameObject.FindGameObjectsWithTag("starterBorder");
            GameObject[] pair1Border = GameObject.FindGameObjectsWithTag("pair1Border");
            GameObject[] pair2Border = GameObject.FindGameObjectsWithTag("pair2Border");


            List<GameObject> firstHalfOfCards = new List<GameObject>();

            for (int j = 0; j < 10; j++)
            {
                firstHalfOfCards.Add(parent[j]);
            }

            ListShuffle.Shuffle(firstHalfOfCards);

            for (int j = 0; j < 10; j++)
            {
                starterBorder[j].GetComponent<Border>().transform.position = new Vector3((float)x, Constants.yPairMid, 0);
                starterBorder[j].GetComponent<Border>().tag = "Border";
                starterBorder[j].GetComponent<Border>().setOccupied(true);
                starterBorder[j].GetComponent<Border>().setCard(firstHalfOfCards[j].GetComponentInChildren<CardModel>());
                starterBorder[j].GetComponent<Border>().setId(firstHalfOfCards[j].GetComponentInChildren<CardModel>().getUniqueCardId());

                pair1Border[j].GetComponent<Border>().transform.position = new Vector3((float)x, Constants.yPairTemp, 0);

                pair2Border[j].GetComponent<Border>().transform.position = new Vector3((float)x, Constants.yPairUpper, 0);
                pair2Border[j].GetComponent<Border>().tag = "Border";

                firstHalfOfCards[j].transform.position = new Vector3((float)x, Constants.yPairMid, 0);
                firstHalfOfCards[j].GetComponentInChildren<CardModel>().setBorder(starterBorder[j].GetComponent<Border>());
                firstHalfOfCards[j].GetComponentInChildren<DragAndDrop>().setDrag(true);

                x += Constants.padding + Constants.cardSize;
            }
        }
    }

    public void giveNextCard()
    {
        UpdateTaskMessage(Constants.pairTaskHelp5);
        imReadyButtonPair1.gameObject.SetActive(false);
        pairGameSecondCollectionOfCardsArePlayable = true;
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
        int counter = 0;
        for (int i = 0; i < borders.Length; i++)
        {
            if (borders[i].transform.position.y == Constants.yPairUpper && borders[i].GetComponent<Border>().getOccupied() == true)
            {
                counter++;
            }
        }

        if (counter == 10)
        {
            camera.GetComponent<CameraMovement>().setMovement(0, -3.0f, -15f, true, 0.5f);

            GameObject[] pair1Border = GameObject.FindGameObjectsWithTag("pair1Border");
            GameObject[] parent = GameObject.FindGameObjectsWithTag("Parent");

            for (int i = 0; i < borders.Length; i++)
            {
                if (borders[i].GetComponent<Border>().getIsStarter() != true)
                {
                    borders[i].GetComponent<Border>().tag = "pair1Border";
                    borders[i].GetComponent<Border>().setIsAvailable(false);
                    borders[i].GetComponent<Border>().getCard().GetComponentInParent<DragAndDrop>().setDrag(false);
                }
                else
                {
                    borders[i].GetComponent<Border>().setIsStarter(false);
                    borders[i].GetComponent<Border>().setIsAvailable(true);
                    borders[i].GetComponent<Border>().setOccupied(false);
                }
            }

            double x = calcx(10, 1);

            List<GameObject> remainingCards = new List<GameObject>();
            for (int i = 0; i < parent.Length; i++)
            {
                if (parent[i].GetComponentInChildren<CardModel>().getBorder() == null)
                {
                    remainingCards.Add(parent[i]);
                }
            }

            ListShuffle.Shuffle(remainingCards);

            for (int i = 0; i < pair1Border.Length; i++)
            {
                pair1Border[i].transform.position = new Vector3((float)x, Constants.yPairLower, 0);
                pair1Border[i].GetComponent<Border>().tag = "Border";
                pair1Border[i].GetComponent<Border>().setCard(remainingCards[i].GetComponentInChildren<CardModel>());
                pair1Border[i].GetComponent<Border>().setId(remainingCards[i].GetComponentInChildren<CardModel>().getUniqueCardId());
                pair1Border[i].GetComponent<Border>().setOccupied(true);
                pair1Border[i].GetComponent<Border>().setIsAvailable(true);
                pair1Border[i].GetComponent<Border>().setIsStarter(true);

                remainingCards[i].transform.position = new Vector3((float)x, Constants.yPairLower, 0);
                remainingCards[i].GetComponentInChildren<CardModel>().setBorder(pair1Border[i].GetComponent<Border>());
                remainingCards[i].GetComponentInChildren<DragAndDrop>().setDrag(true);
                x += Constants.padding + Constants.cardSize;
            }
        }
    }

    public void evaluatePair()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
        GameObject[] firstRowBorders = GameObject.FindGameObjectsWithTag("pair1Border");

        int counter = 0;

        for (int i = 0; i < borders.Length; i++)
        {
            if (borders[i].transform.position.y == Constants.yPairMid && borders[i].GetComponent<Border>().getOccupied() == true)
            {
                counter++;
            }
        }

        int serialScore = 0;
        int sequencialScore = 0;

        if (counter == 10)
        {
            for (int i = 0; i < borders.Length; i++)
            {
                if (borders[i].transform.position.y == Constants.yPairMid)
                {
                    borders[i].tag = "pair2Border";
                }
            }

            GameObject[] secondRowBorders = GameObject.FindGameObjectsWithTag("pair2Border");

            for (int i = 0; i < firstRowBorders.Length; i++)
            {
                if (firstRowBorders[i].GetComponent<Border>().getCard() == orderOfCardsInPairGame[i].GetComponentInChildren<CardModel>())
                {
                    serialScore++;
                }
            }

            for (int i = 0; i < secondRowBorders.Length; i++)
            {
                if (secondRowBorders[i].GetComponent<Border>().getCard() == firstRowBorders[i].GetComponent<Border>().getCard().getPair())
                {
                    sequencialScore++;
                }
            }

            UpdateUserStats(serialScore, sequencialScore);
            //if(API.instance.userId != -1)
            //{
            //    StartCoroutine(API.instance.sendData());
            //}
            StartCoroutine(API.instance.sendData());
            firstRun = true;

            orderPairGameEndDialogue.GetComponent<Animator>().SetBool("CanOpen", true);
            orderResult.text = Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty-1].Score.Score1 + "/10  " + Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty-1].Score.Score1 * 10 + "%";
            pairResult.text = Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty - 1].Score.Score2 + "/10  " + Profile.instance.Custom.data[Profile.instance.Custom.currentDifficulty - 1].Score.Score2 * 10 + "%";
        }
        else
        {
            Debug.Log("Nem minden kártya van még a helyén!");
        }

    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------

    #region NewArrivalGameFunctions
    //Functions for game no. 3 (New Arrival):

    //A játék legvégére az előkészítés, ennek lefutása után tudunk tippelni, hogy melyik az új felszálló
    IEnumerator tippingPosition(float n)
    {
        yield return new WaitForSeconds(n);
        shuffleIndexes(ids1);
        objs = GameObject.FindGameObjectsWithTag("Parent");
        GameObject[] cards = GameObject.FindGameObjectsWithTag("CardModel");
        for (int i = 0; i < objs.Length; i++)
        {
            for (int j = 0; j < ids1.Count; j++)
            {
                if (cards[j].GetComponent<CardModel>().getUniqueCardId() == (int)ids1[i])   //getCardId volt
                {
                    GameObject go = objs[j];
                    go.GetComponent<ParentScript>().reveal();
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        //Engedélyezzük a tippet
        canBeSelected = true;
    }


    //Kisorsolja az új felszállót, eltárolja az indexét majd felteszi a pályára 
    IEnumerator WaitaBitAndAddNewArrival(float n)
    {
        //Beteszünk egy új kártyát miután felfordítottuk a többit, ő lesz az új felszálló
        yield return new WaitForSeconds(n);
        GameObject[] parent = GameObject.FindGameObjectsWithTag("instantiateParent");
        loadAsset.loadNewArrival();
        ids1.Add(idOfNewArrival);
        var card = Instantiate(parent[0], new Vector3(-10, 5, 0), Quaternion.identity);
        card.tag = "Parent";
        card.GetComponentInChildren<CardModel>().tag = "CardModel";
        card.GetComponentInChildren<CardModel>().rend.materials[1].mainTexture = textures1[textures1.Count - 1];

        //Color c = new Color(0.0f, 0.9f, 0.0f, 1.0f);   Cheatcode
        //card.GetComponentInChildren<CardModel>().rend.materials[1].color = c;
        
        card.GetComponentInChildren<CardModel>().setUniqueCardId(idOfNewArrival);
        card.GetComponentInChildren<CardModel>().setCardValue(cardManager.containerOfCards1[cardManager.containerOfCards1.Count - 1].getCardValue());

        GameObject[] cards = GameObject.FindGameObjectsWithTag("CardModel");

        camera.GetComponent<CameraMovement>().setMovement(0, -1.05f, cameraZPosNewArrival[cards.Length - 2], true, 0.5f);

        positionForSmoothStep(card, 0, 0, gameLevel * -0.2f, true, Constants.speedOfArrivalZeroing);
    }

    IEnumerator waitAndPositionAllCardsInZero(float n)
    {
        yield return new WaitForSeconds(n);

        UpdateTaskMessage(Constants.newArrivalTaskHelp2);

        float z = 0;
        for (int i = 0; i < gameLevel; i++)
        {
            GameObject[] parent = GameObject.FindGameObjectsWithTag("Parent");

            positionForSmoothStep(parent[i], 0, 0, z, true, Constants.speedOfZeroing);

            z -= 0.2f;
        }
    }

    //Az új felszálló után megkever mindent és kiteszi őket elrendezve
    IEnumerator waitABitAndDoAll(float n)
    {
        yield return new WaitForSeconds(n);

        // 1. lépés Megkeressük az összes kártyát
        GameObject[] parent = GameObject.FindGameObjectsWithTag("Parent");

        // 2. lépés Átrendezzük a kártyák sorrendjét
        shuffleIndexes(ids1);

        //3. Pozícionáljuk a kártyákat
        positionCards2(parent);

        UpdateTaskMessage(Constants.newArrivalTaskHelp3);
    }

    //Felfordítja a lapokat egyesével kettő másodpercre majd vissza
    IEnumerator waitABitAndReveal(float n, ParentScript ps)
    {
        if (ps != null)
        {
            yield return new WaitForSeconds(n);
            ps.revealAndHide();
        }
    }

    //Az elején elrendezi a kártyákat
    private void startPositioning(GameObject[] parent, float xOffset, float yOffset, float zOffset)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        for (int i = 0; i < parent.Length; i++)
        {
            double val = parent.Length;
            parent[i].transform.position = new Vector3(x, y, z);
            x -= xOffset;
            y -= yOffset;
            z -= zOffset;

            if (i == Math.Ceiling(val))
            {
                x = 0;
                y = 0;
                z = 0;
                xOffset *= -1;
                yOffset *= -1;
                zOffset *= -1;
            }
        }
    }

    private void positionCards2(GameObject[] objs)
    {
        double xUpper = 0;
        double xLower = 0;
        double yUpper = 0;
        double yLower = 0;

        yUpper = Constants.yUp;
        yLower = Constants.yDown;

        if (objs.Length < 6)
        {
            xUpper = calcx(objs.Length, 1);
            yUpper = Constants.yMid;
            pos2(objs, xUpper, yUpper, 0, 0);
        }
        else
        {
            if (objs.Length % 2 == 0)
            {
                xUpper = calcx(objs.Length / 2, 1);
                xLower = xUpper;
                pos2(objs, xUpper, yUpper, xLower, yLower);
            }
            else
            {
                double val = objs.Length;
                xUpper = calcx(Math.Ceiling(val / 2), 1);
                xLower = calcx(Math.Floor(val / 2), 1);
                pos2(objs, xUpper, yUpper, xLower, yLower);
            }
        }
    }

    private void pos2(GameObject[] objs, double xUpper, double yUpper, double xLower, double yLower)
    {
        double cardCount = objs.Length;
        for (int i = 0; i < cardCount; i++)
        {
            for (int j = 0; j < ids1.Count; j++)
            {
                if (objs[j].GetComponentInChildren<CardModel>().getUniqueCardId() == (int)ids1[i])   //getCardID volt
                {
                    Vector3 b = new Vector3((float)xUpper, (float)yUpper, 0);
                    positionForSmoothStep(objs[j], b.x, b.y, b.z, true, Constants.speedOfFirstPositioning);

                    xUpper += Constants.padding + Constants.cardSize;
                    if (objs.Length > 5 && Math.Ceiling(cardCount / 2) - 1 == i)
                    {
                        xUpper = xLower;
                        yUpper = yLower;
                    }
                }
            }
        }
    }
    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------

    #region BasicFunctions
    //Basic funtions that all games use:
    private void loadInObjects()
    {
        GameObject[] temp1 = GameObject.FindGameObjectsWithTag("CardModel");
        for (int i = 0; i < temp1.Length; i++)
        {
            listForMain.Add(temp1[i]);
        }
    }

    public void positionForSmoothStep(GameObject parent, float xEnd, float yEnd, float zEnd, bool canStart, float duration)
    {
        ParentScript parentScript = parent.GetComponent<ParentScript>();

        parentScript.startTime = Time.time;

        parentScript.xStart = parent.transform.position.x;
        parentScript.yStart = parent.transform.position.y;
        parentScript.zStart = parent.transform.position.z;

        parentScript.xEnd = xEnd;
        parentScript.yEnd = yEnd;
        parentScript.zEnd = zEnd;

        parentScript.duration = duration;
        parentScript.canStart = canStart;
        parentScript.t = 0;
    }


    IEnumerator shuffle(GameObject[] parent, float n)
    {
        yield return new WaitForSeconds(n);
        float x = 2 * Constants.cardSize;
        for (int i = 0; i < 4; i++)
        {

            positionForSmoothStep(parent[i], x, parent[i].transform.position.y, (gameLevel + 1 + i) * -0.2f, true, Constants.speedOfThreeShuffles);

            if (FindObjectOfType<SoundButtonScript>().soundOn < 2)
            {
                FindObjectOfType<AudioManager>().Play("Slide", "effect");
            }

            yield return new WaitForSeconds(Constants.speedOfThreeShuffles);

            positionForSmoothStep(parent[i], 0, 0, parent[i].transform.position.z, true, Constants.speedOfThreeShuffles);

            yield return new WaitForSeconds(Constants.speedOfThreeShuffles + 0.1f);
        }
    }


    //Reload bundles if we have clicked on the newAssetBundle button
    public void newBundle()
    {
        if (newAssetDrawer.getDrawNewAssetValue())
        {
            newAssetDrawer.DrawNewAsset();
        }
        else
        {
            Debug.Log("Most nem cseréltünk kártya setet");
        }
    }

    //Egy lista tartalmát megkeveri
    public void shuffleIndexes(ArrayList list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            int value = (int)list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private double calcx(double cardNumber, float scale)
    {
        return -(((cardNumber / 2) - 0.5f) * ((Constants.cardSize * scale) + Constants.padding));
    }

    public GameObject findParentObjectByID(int id)
    {
        GameObject[] parents = GameObject.FindGameObjectsWithTag("Parent");
        foreach (GameObject GO in parents)
        {
            if (GO.GetComponentInChildren<CardModel>().getUniqueCardId() == id)
            {
                return GO;
            }
        }
        return null;
    }

    public void resetGame()
    {
        GameObject[] borderTemp = GameObject.FindGameObjectsWithTag("Border");
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Parent");
        GameObject[] borderTemp2 = GameObject.FindGameObjectsWithTag("pair2Border");
        GameObject[] borderTemp1 = GameObject.FindGameObjectsWithTag("pair1Border");

        for (int i = 0; i < temp.Length; i++)
        {
            DestroyImmediate(temp[i]);
        }

        cardManager.containerOfCards1.Clear();
        listForMain.Clear();
        ids1.Clear();
        textures1.Clear();

        wonMessage.GetComponent<Animator>().SetBool("CanOpen", false);
        lostMessage.GetComponent<Animator>().SetBool("CanOpen", false);
        orderPairGameEndDialogue.GetComponent<Animator>().SetBool("CanOpen", false);

        switch (API.instance.data.chosenGameMode)
        {
            //New Arrival
            case 1:
                waitTimeForReveal = waitTimeForRevealReference;
                canBeSelected = false;
                break;

            //PairGame
            case 2:
                for (int j = 0; j < borderTemp.Length; j++)
                {
                    DestroyImmediate(borderTemp[j]);
                }

                for (int j = 0; j < borderTemp2.Length; j++)
                {
                    DestroyImmediate(borderTemp2[j]);
                    DestroyImmediate(borderTemp1[j]);
                }

                imReadyButtonPair2.gameObject.SetActive(false);
                pairGameOrderIndicator.GetComponentInChildren<Text>().text = "1";
                pairGameSecondCollectionOfCardsArePlayable = false;
                cardManager.containerOfCards2.Clear();
                ids2.Clear();
                textures2.Clear();
                counterForPairGame = 0;
                orderOfCardsInPairGame.Clear();
                break;

            //OrderGame
            case 3:
                imReadyButtonOrder.gameObject.SetActive(false);
                orderGameCheat.text = "";
                orderedIds.Clear();
                for (int j = 0; j < borderTemp.Length; j++)
                {
                    DestroyImmediate(borderTemp[j]);
                }
                break;
            default:
                Debug.Log("No such case as given");
                break;
        }

        newAssetDrawer.DrawNewAsset();
    }

    public void lockCards()
    {
        canBeSelected = false;
    }

    public void guessedRight(bool right)
    {
        if (right)
        {
            wonMessage.GetComponent<Animator>().SetBool("CanOpen", true);

            rightGuesses += 1;
            wrongGuesses = 0;
            if (rightGuesses == 2)
            {
                rightGuesses = 0;                //Emeljük a tétet
                if (API.instance.data.chosenGameMode == 1) //NewArrival
                {
                    if (gameLevel < Constants.maxLevelOfNewArrival)
                    {
                        gameLevel++;
                    }
                }
                else
                {
                    if (gameLevel < Constants.maxLevelOfOrderGame)
                    {
                        gameLevel++;
                    }
                }
            }
        }
        else
        {
            lostMessage.GetComponent<Animator>().SetBool("CanOpen", true);
            wrongGuesses += 1;
            rightGuesses = 0;
            if (wrongGuesses == 2)
            {
                wrongGuesses = 0;    //Csökkentjük a tétet
                if (API.instance.data.chosenGameMode == 1) //NewArrival
                {
                    if (gameLevel > Constants.minLevelOfNewArrival)
                    {
                        gameLevel--;
                    }
                }
                else
                {
                    if (gameLevel > Constants.minLevelOfOrderGame)
                    {
                        gameLevel--;
                    }
                }
            }
        }

        UpdateUserStats(gameLevel, 0);

        //if(API.instance.userId != -1)
        //{
        //    StartCoroutine(API.instance.sendData());
        //}
        StartCoroutine(API.instance.sendData());
        firstRun = true;
    }

    private void UpdateUserStats(int score1, int score2)
    {
        Profile.instance.SetDataScore(new Score(score1, score2));
    }

    private void UpdateTaskMessage(string text)
    {
        taskMessage.text = text;
    }

    private IEnumerator HoldGame(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        if (firstGameOfTheDay)
        {
            Time.timeScale = 0.0f;
            firstGameOfTheDay = false;
        }
    }


#endregion

    //----------------------------------------------------------------------------------------------------------------------------------

#region Getters / Setters
    //Getters and setters for variables:
    public bool getCanBeSelected()
    {
        return canBeSelected;
    }
    public void setIdOfNewArrival(int id)
    {
        idOfNewArrival = id;
    }
    public int getGameLevel()
    {
        return gameLevel;
    }
    public int getIdOfNewArrival()
    {
        return idOfNewArrival;
    }

#endregion
}