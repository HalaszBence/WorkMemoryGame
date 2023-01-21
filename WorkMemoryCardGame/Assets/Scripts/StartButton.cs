using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1 && GameController.instance.firstRun == true && GameController.instance.textures1.Count > 0 )
        {
            GameController.instance.resetGame();
            GameController.instance.firstRun = false;
        }
    }
}
