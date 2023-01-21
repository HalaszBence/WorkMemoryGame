using UnityEngine;

public class FullScreenButton : MonoBehaviour
{
    public void ChangeFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
