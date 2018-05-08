using UnityEngine;
using System.Collections;

// This class is used to fade in and out groups of UI elements.  
// It contains a variety of functions for fading in different ways.
[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    [Tooltip("The amount the alpha of the UI elements changes per second.")]
    [SerializeField]
    private float _fadeSpeed = 1f;

    /// <summary>
    /// All the groups of UI elements that will fade in and out.
    /// </summary>
    [Tooltip("")]
    [SerializeField]
    private CanvasGroup _groupToFade;

    [Tooltip("Should the UI elements be visible to start?")]
    [SerializeField]
    private bool _startVisible;                 // 

    [Tooltip("Should the UI elements begin fading with they start up? Fading can either be in or out (opposite of their starting alpha)")]
    [SerializeField]
    private bool _startWithFade;

    /// <summary>
    /// Whether the UI elements are currently visible.
    /// </summary>
    private bool _visible;


    void Reset()
    {
        //Attempt to grab the CanvasGroup on this object
        _groupToFade = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        //If the object should start visible, set it to be visible. Otherwise, set it invisible
        if (_startVisible)
            SetVisible ();
        else
            SetInvisible ();

        //If there shouldn't be any initial fade, leave this method
        if (!_startWithFade)
            return;

        //If the object is currently visible, fade out. Otherwise fade in
        if (_visible)
            StartFadeOut ();
        else
            StartFadeIn ();
    }

    //Publicly accessible methods for fading in or fading out without needing to start a 
    //coroutine. These are needed in order for UI events (like buttons) to start a fade in
    //or out.
    public void StartFadeIn()
    {
        StartCoroutine (FadeIn ());
    }

    public void StartFadeOut()
    {
        StartCoroutine (FadeOut ());
    }

    // These functions are used if fades are required to be instant.
    public void SetVisible ()
    {
        _groupToFade.alpha = 1f;
        _visible = true;
    }


    public void SetInvisible ()
    {
        _groupToFade.alpha = 0f;
        _visible = false;
    }

    IEnumerator FadeIn()
    {
        // Fading needs to continue until the group is completely faded in
        while (_groupToFade.alpha < 1f) 
        {
            //Increase the alpha
            _groupToFade.alpha += _fadeSpeed * Time.deltaTime;
            //Wait a frame
            yield return null;
        }

        // Since everthing has faded in now, it is visible.
        _visible = true;
    }

    IEnumerator FadeOut ()
    {
        while (_groupToFade.alpha > 0f) 
        {
            _groupToFade.alpha -= _fadeSpeed * Time.deltaTime;

            yield return null;
        }

        _visible = false;
    }

    public void Flash()
    {
        StartCoroutine (ProcessFlash ());
    }

    IEnumerator ProcessFlash()
    {
        yield return StartCoroutine (FadeIn ());
        yield return StartCoroutine (FadeOut ());
    }
}