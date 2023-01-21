using System.Collections;
using UnityEngine;

public class ParentScript : MonoBehaviour
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
    private Animator anim;

    #endregion

    void Update()
    {
        if(canStart == true && t <= 1.0f)
        {
            t = (Time.time - startTime) / duration;
            transform.position = new Vector3(Mathf.SmoothStep(xStart, xEnd, t), Mathf.SmoothStep(yStart, yEnd, t), Mathf.SmoothStep(zStart, zEnd, t));
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void reveal()
    {
        if (anim != null)
        {
            anim.SetBool("triggerFlag", true);
        }
    }

    public void revealAndHide()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("triggerFlag", true);
        if (FindObjectOfType<SoundButtonScript>().soundOn < 2)
        {
            FindObjectOfType<AudioManager>().Play("Turn", "effect");
        }
        StartCoroutine(turnBack());
    }

    IEnumerator turnBack()
    {
        yield return new WaitForSeconds(2.0f);
        anim.SetBool("triggerFlag", false);
        StartCoroutine(turnOriginal());
        if (FindObjectOfType<SoundButtonScript>().soundOn < 2)
        {
            FindObjectOfType<AudioManager>().Play("Turn", "effect");
        }
    }

    IEnumerator turnOriginal()
    {
        yield return new WaitForSeconds(2.0f);
        anim.SetBool("goBack", true);
    }
}
