using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas canvasInstance;

    [Header("Component References")]

    [SerializeField]
    private Text _gameStatus;
    [SerializeField]
    private Text _killsValue;
    [SerializeField]
    private Text _experienceValue;
    [SerializeField]
    private Text _ammoValue;

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
        SetAmmo(0);
        SetKills(0);
        SetExperience(0);
        ClearGameStatusDisplay();
    }

    public void FlashDamageEffect()
    {
        //damageImage.Flash();
    }

    public void PlayDeathAudio()
    {
        //if (!deathAudio.isPlaying)
        //    deathAudio.Play();
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
        Invoke("ClearGameStatusDisplay", 2f);
    }

    public void ClearGameStatusDisplay()
    {
        DisplayGameStatus("");
    }
}