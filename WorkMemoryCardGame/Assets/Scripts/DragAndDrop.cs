using UnityEngine.EventSystems;
using UnityEngine;
using System;
using System.Collections.Generic;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region Variables
    private Vector3 _dragOffset;
    private Camera _cam;
    private Vector3 startingPos;

    [SerializeField] private int borderNumber = 0;
    [SerializeField] private bool canDrag = false;
    [SerializeField] private float _speed = 100;
    #endregion

    void Awake()
    {
        _cam = Camera.main;
    }

    public void setDrag(bool val)
    {
        canDrag = val;
    }

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.allCameras[0].ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.back, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }


    Vector3 GetMousePos(float x)
    {
        Vector3 worldPos = GetWorldPositionOnPlane(Input.mousePosition, x);
        return worldPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            startingPos = transform.position;
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
            _dragOffset = transform.position - GetMousePos(0.0f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            transform.position = Vector3.MoveTowards(transform.position, GetMousePos(-0.1f) + _dragOffset, _speed * Time.deltaTime);
        }
    }

    private Border findNewPlace()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
        for (int i = 0; i < borders.Length; i++)
        {
            if (!borders[i].GetComponent<Border>().getOccupied() && borders[i].GetComponent<Border>().getIsStarter())
            {
                return borders[i].GetComponent<Border>();
            }
        }
        return null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Border newBorder = findNewPlace();

        if (canDrag)
        {
            GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");
            GameObject[] cards = GameObject.FindGameObjectsWithTag("Parent");

            if (calculateDistance() < 1.0f && this.GetComponentInChildren<CardModel>().getBorder() != borders[borderNumber].GetComponent<Border>())
            {
                Border oldBorder = this.GetComponentInChildren<CardModel>().getBorder();
                Border newestBorder = borders[borderNumber].GetComponent<Border>();

                if (newestBorder.getCard() == null && newestBorder.getIsAvailable())
                {
                    Debug.Log("Case 1");

                    //Old border:
                    oldBorder.setCard(null);
                    oldBorder.setId(-1);
                    oldBorder.setOccupied(false);

                    //Card settings:
                    this.GetComponentInChildren<CardModel>().setBorder(newestBorder); 
                    this.GetComponentInParent<ParentScript>().transform.position = newestBorder.transform.position;

                    //New border settings:
                    newestBorder.setCard(this.GetComponentInChildren<CardModel>());
                    newestBorder.setId(this.GetComponentInChildren<CardModel>().getUniqueCardId());
                    newestBorder.setOccupied(true);
                }
                else
                {
                    if (newBorder == null && newestBorder.getOccupied() != false)
                    {
                        Debug.Log("Case 2");
                        //Old border:
                        oldBorder.setCard(newestBorder.getCard());
                        oldBorder.setId(newestBorder.getCard().GetComponent<CardModel>().getUniqueCardId());

                        //Old card:
                        newestBorder.getCard().setBorder(oldBorder);
                        Vector3 destination = oldBorder.GetComponent<Border>().transform.position;
                        for (int i = 0; i < cards.Length; i++)
                        {
                            if (cards[i].GetComponentInChildren<CardModel>().getUniqueCardId() == newestBorder.getCard().GetComponent<CardModel>().getUniqueCardId())
                            {
                                GameController.instance.positionForSmoothStep(cards[i], destination.x, destination.y, 0, true, Constants.cardChangeSpeed);
                            }
                        }

                        //New border:
                        newestBorder.setCard(this.GetComponentInChildren<CardModel>());
                        newestBorder.setId(this.GetComponentInChildren<CardModel>().getUniqueCardId());

                        //New card:
                        this.GetComponentInChildren<CardModel>().setBorder(newestBorder);
                        Vector3 dest = borders[borderNumber].transform.position;
                        for (int i = 0; i < cards.Length; i++)
                        {
                            if (cards[i].GetComponentInChildren<CardModel>().getUniqueCardId() == this.GetComponentInChildren<CardModel>().getUniqueCardId())
                            {
                                GameController.instance.positionForSmoothStep(cards[i], dest.x, dest.y, dest.z, true, Constants.cardChangeSpeed);
                            }
                        }
                    }
                    else
                    {
                        if (newestBorder.getOccupied() == true)
                        {
                            Debug.Log("Case 3");

                            //Settings for the border where the card went:
                            newBorder.GetComponent<Border>().setCard(newestBorder.getCard().GetComponent<CardModel>());
                            newBorder.GetComponent<Border>().setOccupied(true);
                            newBorder.GetComponent<Border>().setId(newestBorder.getCard().GetComponent<CardModel>().getUniqueCardId());

                            //Old cards settings:
                            newestBorder.getCard().GetComponent<CardModel>().setBorder(newBorder);
                            Vector3 destination2 = newBorder.transform.position;
                            for (int i = 0; i < cards.Length; i++)
                            {
                                if (cards[i].GetComponentInChildren<CardModel>().getUniqueCardId() == borders[borderNumber].GetComponent<Border>().getCard().GetComponent<CardModel>().getUniqueCardId())
                                {
                                    GameController.instance.positionForSmoothStep(cards[i], destination2.x, destination2.y, destination2.z, true, Constants.cardChangeSpeed);
                                }
                            }
                        }

                        Debug.Log("Case 4");
                        //Old border settings:
                        if (oldBorder != null)
                        {
                            oldBorder.setOccupied(false);
                            oldBorder.setCard(null);
                            oldBorder.setId(-1);
                        }

                        //New card settings
                        Vector3 dest = borders[borderNumber].transform.position;
                        for (int i = 0; i < cards.Length; i++)
                        {
                            if (cards[i].GetComponentInChildren<CardModel>().getUniqueCardId() == this.GetComponentInChildren<CardModel>().getUniqueCardId())
                            {
                                GameController.instance.positionForSmoothStep(cards[i], dest.x, dest.y, dest.z, true, Constants.cardChangeSpeed);
                            }
                        }
                        this.GetComponentInChildren<CardModel>().setBorder(newestBorder);

                        newestBorder.setOccupied(true);
                        newestBorder.setId(this.GetComponentInChildren<CardModel>().getUniqueCardId());
                        newestBorder.setCard(this.GetComponentInChildren<CardModel>());
                    }
                }
            }
            else
            {
                this.GetComponentInParent<ParentScript>().transform.position = startingPos;
            }
            ShowImReadyButton();
        }
    }

    public double calculateDistance()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");

        double[] distances = new double[borders.Length];
        for (int i = 0; i < borders.Length; i++)
        {
            float a = (transform.position.x - borders[i].transform.position.x) * (transform.position.x - borders[i].transform.position.x);
            float b = (transform.position.y - borders[i].transform.position.y) * (transform.position.y - borders[i].transform.position.y);
            if (borders[i].GetComponent<Border>().getIsAvailable() == true)
            {
                distances[i] = Math.Abs(Math.Sqrt(a * a + b * b));
            }
            else
            {
                distances[i] = 100000;
            }
        }

        double Min = 100000;
        int k = 0;
        foreach (double number in distances)
        {
            if (number < Min)
            {
                Min = number;
                borderNumber = k;
            }
            k++;
        }
        return Min;
    }

    private void ShowImReadyButton()
    {
        switch (API.instance.data.chosenGameMode)
        {
            case 1:
                Debug.Log("We have nothing to do");
                break;
            case 2:
                GameObject[] bordersPair = GameObject.FindGameObjectsWithTag("Border");
                GameObject[] tempBorders = GameObject.FindGameObjectsWithTag("pair1Border");

                List<GameObject> importantBorders1 = new List<GameObject>();
                List<GameObject> importantBorders2 = new List<GameObject>();
                List<GameObject> importantBorders3 = new List<GameObject>();
                
                
                foreach(GameObject GO in tempBorders)
                {
                    if(GO.transform.position.y == Constants.yPairTemp)
                    {
                        importantBorders3.Add(GO);
                    }
                }
                
                foreach (GameObject GO in bordersPair)
                {
                    if (GO.transform.position.y == Constants.yPairUpper)
                    {
                        importantBorders1.Add(GO);
                    }
                    else if(GO.transform.position.y == Constants.yPairMid)
                    {
                        importantBorders2.Add(GO);
                    }
                }

                bool canShowReady1 = true;
                bool canShowReady2 = true;

                foreach (GameObject GO in importantBorders1)
                {
                    if (GO.GetComponent<Border>().getOccupied() != true)
                    {
                        canShowReady1 = false;
                    }
                }


                foreach (GameObject GO in importantBorders2)
                {
                    if (GO.GetComponent<Border>().getOccupied() != true)
                    {
                        canShowReady2 = false;
                    }
                }

                if(canShowReady1 && importantBorders3.Count != 0 && GameController.instance.pairGameSecondCollectionOfCardsArePlayable == false)
                {
                    GameController.instance.imReadyButtonPair1.gameObject.SetActive(true);
                }
                else
                {
                    GameController.instance.imReadyButtonPair1.gameObject.SetActive(false);
                }

                if(canShowReady2 == true && importantBorders2.Count > 0 && importantBorders3.Count == 0)
                {
                    GameController.instance.imReadyButtonPair2.gameObject.SetActive(true);
                } 
                else
                {
                    GameController.instance.imReadyButtonPair2.gameObject.SetActive(false);
                }

                break;
            case 3:
                GameObject[] bordersOrder = GameObject.FindGameObjectsWithTag("Border");
                List<GameObject> importantBorders = new List<GameObject>();
                foreach(GameObject GO in bordersOrder)
                {
                    if(GO.transform.position.y == Constants.yOrderBorder)
                    {
                        importantBorders.Add(GO);
                    }
                }
                bool canShow = true;
                foreach(GameObject GO in importantBorders)
                {
                    if(GO.GetComponent<Border>().getOccupied() != true)
                    {
                        canShow = false;
                    }
                }
                if(canShow)
                {
                    GameController.instance.imReadyButtonOrder.gameObject.SetActive(true);
                }
                else
                {
                    GameController.instance.imReadyButtonOrder.gameObject.SetActive(false);
                }
                break;
            default:
                Debug.Log("No such case as given");
                break;
        }
    }
}