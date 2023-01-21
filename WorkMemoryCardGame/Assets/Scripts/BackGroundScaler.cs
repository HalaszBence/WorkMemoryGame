using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScaler : MonoBehaviour
{
    public void SetScale(float scaler)
    {
        gameObject.transform.localScale = new Vector3(scaler, scaler, scaler);
    }
}
