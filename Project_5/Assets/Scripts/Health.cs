using System;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    // TODO: allow BasicObjectController to change max health (e.g. increase in level)
    public uint MaxHealth = 100;

    [Tooltip("Health bar visible to user")]
    [SerializeField]
    private RectTransform HealthBar;

    [SyncVar(hook = "UpdateHealthBar")]
    [HideInInspector]
    public int CurrentHealth = HealthBarLength; // just some default; will be updated in Awake

    /// <summary>
    /// the length of the health bar (in pixels or whatever)
    /// </summary>
    private const int HealthBarLength = 100;

    void Awake()
    {
        CurrentHealth = (int)MaxHealth;
    }

    /// <summary>
    /// reduce the health by the given amount
    /// Only applied on server, then changes are then synchronized on the Clients.
    /// </summary>
    public void TakeDamage(uint amount)
    {
        // only apply damage on server
        if (!isServer || amount == 0)
            return;

        // limit the number of times SyncVar is set to limit chatter
        if(CurrentHealth - (int)amount <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("Dead");
            var controller = this.GetComponent<BasicObjectController>();
            if(controller != null)
                controller.OnZeroHealth();
        }
        else
            CurrentHealth -= (int)amount;
        //UpdateHealthBar(CurrentHealth);
    }

    /// <summary>
    /// increase the current health by the given amount
    /// Only applied on server, then changes are then synchronized on the Clients.
    /// </summary>
    public void TakeHeal(uint amount)
    {
        // only apply damage on server
        if (!isServer)
            return;

        // add to health, but keep max health threshold
        CurrentHealth = (int)Math.Min(CurrentHealth + (int)amount, MaxHealth);

        //UpdateHealthBar(CurrentHealth);
    }

    /// <summary>
    /// change max health to new value
    /// Only applied on server, then changes are then synchronized on the Clients.
    /// </summary>
    public void UpdateMaxHealth(uint newMaxHealthAmount)
    {
        MaxHealth = newMaxHealthAmount;

        //UpdateHealthBar(CurrentHealth);
    }
    /// <summary>
    /// update the health bar to reflect Current Health (health remaining)
    /// </summary>
    private void UpdateHealthBar(int currentHealth)
    {
        // calculate the how the current health bar should look, using the max health
        int currentHealthBarLength = (int)(((float)HealthBarLength / (float)MaxHealth) * (float)currentHealth);
        HealthBar.sizeDelta = new Vector2(currentHealthBarLength, HealthBar.sizeDelta.y);
    }
}