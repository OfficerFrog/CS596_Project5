using System;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    // TODO: allow BasicObjectController to change max health (e.g. increase in level)
    public int MaxHealth = 100;

    [Tooltip("Health bar visible to user")]
    [SerializeField]
    private RectTransform HealthBar;

    [SyncVar(hook = "UpdateHealthBar")]
    [HideInInspector]
    public int CurrentHealth;

    void Awake()
    {
        CurrentHealth = (int)MaxHealth;
    }

    /// <summary>
    /// reduce the health by the given amount
    /// </summary>
    public void TakeDamage(uint amount)
    {
        // only apply damage on server
        if (!isServer || amount == 0)
            return;

        CurrentHealth -= (int)amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("Dead");
            var controller = this.GetComponent<BasicObjectController>();
            if(controller != null)
                controller.OnZeroHealth();
        }
        UpdateHealthBar();
    }

    /// <summary>
    /// increase the current health by the given amount
    /// </summary>
    public void TakeHeal(uint amount)
    {
        // only apply damage on server
        if (!isServer)
            return;

        // add to health, but keep max health threshold
        CurrentHealth = (int)Math.Min(CurrentHealth += (int)amount, MaxHealth);

        UpdateHealthBar();
    }

    /// <summary>
    /// change max health to new value
    /// </summary>
    public void UpdateMaxHealth(int newMaxHealthAmount)
    {
        MaxHealth = newMaxHealthAmount;

        UpdateHealthBar();
    }
    /// <summary>
    /// update the health bar to reflect Current Health (health remaining)
    /// </summary>
    private void UpdateHealthBar()
    {
        HealthBar.sizeDelta = new Vector2(CurrentHealth, HealthBar.sizeDelta.y);
    }
}