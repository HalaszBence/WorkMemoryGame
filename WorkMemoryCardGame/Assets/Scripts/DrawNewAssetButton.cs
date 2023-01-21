using UnityEngine;

public class DrawNewAssetButton : MonoBehaviour
{
    #region Variables
    [SerializeField] private CardSetManager cardSetManager;
    [SerializeField] private bool hasNewLoaded = false;
    //[SerializeField] private StartButton startButton;
    [SerializeField] private LoadAsset loadAsset;
    #endregion

    public void Awake()
    {
        cardSetManager = GameObject.FindGameObjectWithTag("cardSetCollectionManager").GetComponent<CardSetManager>();
    }

    public void drawAssetClicked()
    {
        hasNewLoaded = true;
    }

    public void DrawNewAsset()
    {
        cardSetManager = GameObject.FindGameObjectWithTag("cardSetCollectionManager").GetComponent<CardSetManager>();

        string newAsset1;
        string newAsset2;

        if (API.instance.data.chosenGameMode == 2)
        {
            newAsset1 = cardSetManager.drawAsset();
            newAsset2 = cardSetManager.drawAsset();

            while(newAsset2 == newAsset1)
            {
                newAsset2 = cardSetManager.drawAsset();
            }
            
            GameController.instance.assetName1 = newAsset1;
            GameController.instance.assetName2 = newAsset2;

            loadAsset.unloadAsset();
            loadAsset.loadAssetBundles(newAsset1, newAsset2);
        }
        else
        {
            newAsset1 = cardSetManager.drawAsset();
            GameController.instance.assetName1 = newAsset1;
            loadAsset.unloadAsset();
            loadAsset.loadAssetBundle(newAsset1);

        }

        Debug.Log("A sorsolas soran a szint: " + GameController.instance.GameLevel); 
    }

    public bool getDrawNewAssetValue()
    {
        return hasNewLoaded;
    }

    public void setDrawNewAssetValue(bool value)
    {
        hasNewLoaded = value;
    }
}
