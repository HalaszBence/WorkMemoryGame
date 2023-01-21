using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Variables
    public float startTime = 0.0f;
    public float duration = 0.01f;

    public float xStart;
    public float yStart;
    public float zStart;

    public float xEnd;
    public float yEnd;
    public float zEnd;
    public bool canStart = false;
    public float t = 0;
    #endregion

    private void Update()
    {
        if(canStart)
        {
            canStart = false;
            StartCoroutine(LerpFromTo(transform.position, new Vector3(xEnd, yEnd, zEnd), duration));
        }
    }

    public IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        for (float t = 0f; t <= duration; t += Time.deltaTime)
        {
            transform.position = new Vector3(Mathf.SmoothStep(pos1.x, pos2.x, t / duration), Mathf.SmoothStep(pos1.y, pos2.y, t / duration), Mathf.SmoothStep(pos1.z, pos2.z, t / duration));
            yield return 0;
        }
        transform.position = pos2;
    }

    public void setMovement(float _xEnd, float _yEnd, float _zEnd, bool _canStart, float _duration)
    {
        xEnd = _xEnd;
        yEnd = _yEnd;
        zEnd = _zEnd;
        canStart = _canStart;
        duration = _duration;
    }
}
