using UnityEngine;
using UnityEngine.UI;

public class SoundButtonScript : MonoBehaviour
{
    public int soundOn = 0;   //0: minden megy 1: csak a hang effekt megy 2: semmi nem megy
    public Sprite[] sprites;
    public Image soundButton;

    public void OnMouseDown()
    {
        if (soundOn == 0)
        {
            AudioManager am = FindObjectOfType<AudioManager>();
            foreach (Sound s in am.sounds)
            {
                if (s.name == "BackGround")
                {
                    s.source.Stop();
                }
            }
            soundButton.sprite = sprites[2];
            soundOn = 1;
        }
        else if(soundOn == 1)
        {
            soundOn = 2;
            soundButton.sprite = sprites[0];
        }
        else
        {
            soundOn = 0;
            soundButton.sprite = sprites[1];
            FindObjectOfType<AudioManager>().Play("BackGround", "music");
        }
    }

    public void OnMouseEnter()
    {
        GameObject.FindGameObjectWithTag("InformationHandler").GetComponent<InformationHandler>().ShowSoundInformation();
    }

    public void OnMouseExit()
    {
        GameObject.FindGameObjectWithTag("InformationHandler").GetComponent<InformationHandler>().HideSoundInformation();
    }
}
