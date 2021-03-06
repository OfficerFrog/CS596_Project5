﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas canvasInstance;

    [Header("Component References")]

    [SerializeField]
    private Text _gameStatus;
    [SerializeField]
    private Text _healthValue;
    [SerializeField]
    private Text _killsValue;
    [SerializeField]
    private Text _experienceValue;
    [SerializeField]
    private Text _ammoValue;

    [SerializeField]
    private UIFader _damageImage;
    //[SerializeField] UIFader damageImage;
    //[SerializeField] AudioSource deathAudio;

    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        if (canvasInstance == null)
            canvasInstance = this;
        else if (canvasInstance != this)
            Destroy(gameObject);
    }

    public void Initialize()
    {
        SetHealth(0);
        SetAmmo(0);
        SetKills(0);
        SetExperience(0);
        ClearGameStatusDisplay();
    }

    public void FlashDamageEffect()
    {
        _damageImage.Flash();
    }

    public void PlayDeathAudio()
    {
        //if (!deathAudio.isPlaying)
        //    deathAudio.Play();
    }

    public void SetHealth(int amount)
    {
        _healthValue.text = amount.ToString();
    }

    public void SetKills(int amount)
    {
        _killsValue.text = amount.ToString();
    }

    public void SetExperience(int amount)
    {
        _experienceValue.text = amount.ToString();
    }

    public void SetAmmo(int amount)
    {
        _ammoValue.text = amount.ToString();
    }

    public void DisplayGameStatus(string text)
    {
        _gameStatus.text = text;
        // only need to clear if there is something to clear
        if(!string.IsNullOrEmpty(text))
            Invoke("ClearGameStatusDisplay", 2f);
    }

    public void ClearGameStatusDisplay()
    {
        DisplayGameStatus("");
    }
}