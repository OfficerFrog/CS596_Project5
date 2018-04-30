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

    [SyncVar(hook = "OnHealthChanged")]
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
    [Server]
    public void TakeDamage(BasicPlayerController shootingPlayer,  uint amount)
    {
        if (amount == 0)
            return;

        // limit the number of times SyncVar is set to limit chatter
        if(CurrentHealth - (int)amount <= 0)
        {
            // object is dead
            CurrentHealth = 0;// remove dead object
            var shotObject = this.GetComponent<DismissibleObjectController>();
            if (shotObject != null)
            {
                // alert shooter
                shootingPlayer.ObjectDestroyed(shotObject);
                // alert shot object
                shotObject.OnZeroHealth();
            }
        }
        else
            CurrentHealth -= (int)amount;
    }

    /// <summary>
    /// increase the current health by the given amount
    /// Only applied on server, then changes are then synchronized on the Clients.
    /// Returns True if was a gain in health
    /// </summary>
    [Server]
    public bool TakeHeal(uint amount)
    {
        if (CurrentHealth >= MaxHealth)
            return false;
        // add to health, but keep within max health threshold
        CurrentHealth = (int)Math.Min(CurrentHealth + (int)amount, MaxHealth);
        return true;
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
    private void OnHealthChanged(int currentHealth)
    {
        // calculate the how the current health bar should look, using the max health
        int currentHealthBarLength = (int)(((float)HealthBarLength / (float)MaxHealth) * (float)currentHealth);
        HealthBar.sizeDelta = new Vector2(currentHealthBarLength, HealthBar.sizeDelta.y);
    }
}