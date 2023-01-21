using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class GraphMaker : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private void Awake()
    {
        //graphcontainer = transform.find("graphcontainer").getcomponent<recttransform>();

        //list<int> values = new list<int>() { 1, 5, 3, 1, 5, 6, 10, 1, 4, 8 };
        //fillgraph(values);
    }

    private GameObject CreateVertex(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("vertex", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void FillGraph(List<int> values)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = 100f;
        float ySize = 10f;
        GameObject latestVertex = null;
        for(int i = 0; i < values.Count; i++)
        {
            float xPosition = i * xSize + xSize;
            float yPosition = (values[i] / ySize) * graphHeight;
            GameObject gameObject = CreateVertex(new Vector2(xPosition, yPosition));
            if (latestVertex != null)
                ConnectVertices(latestVertex.GetComponent<RectTransform>().anchoredPosition, gameObject.GetComponent<RectTransform>().anchoredPosition);
            latestVertex = gameObject;
        }
    }

    private void ConnectVertices(Vector2 vertexA, Vector2 vertexB)
    {
        GameObject gameObject = new GameObject("VertexConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        Vector2 dir = (vertexB - vertexA).normalized;
        float distance = Vector2.Distance(vertexA, vertexB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = vertexA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
    }
}       
    