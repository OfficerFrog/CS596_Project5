using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas canvasInstance;

    [Header("Component References")]
    //[SerializeField] Image reticule;
    //[SerializeField] UIFader damageImage;
    //[SerializeField] Text gameStatusText;
    [SerializeField]
    private Text _healthValue;
    [SerializeField]
    private Text _killsValue;
    [SerializeField]
    private Text _experienceValue;
    //[SerializeField]
    //Text logText;
    //[SerializeField] AudioSource deathAudio;

    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        if (canvasInstance == null)
            canvasInstance = this;
        else if (canvasInstance != this)
            Destroy(gameObject);
    }

    //Find all of our resources
    //void Reset()
    //{
    //    reticule = GameObject.Find("Reticule").GetComponent();
    //    damageImage = GameObject.Find("DamagedFlash").GetComponent();
    //    gameStatusText = GameObject.Find("GameStatusText").GetComponent();
    //    healthValue = GameObject.Find("HealthValue").GetComponent();
    //    killsValue = GameObject.Find("KillsValue").GetComponent();
    //    logText = GameObject.Find("LogText").GetComponent();
    //    deathAudio = GameObject.Find("DeathAudio").GetComponent();
    //}

    public void Initialize()
    {
        _healthValue.text = "0";
        _killsValue.text = "0";
        _experienceValue.text = "0";
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

    public void SetHealth(int amount)
    {
        _healthValue.text = amount.ToString();
    }

    public void SetExperience(int amount)
    {
        _experienceValue.text = amount.ToString();
    }

    public void WriteGameStatusText(string text)
    {
        //gameStatusText.text = text;
    }
}