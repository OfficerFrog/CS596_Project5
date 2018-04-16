using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    // TODO: allow BasicObjectController to change max health (e.g. increase in level)
    public uint MaxHealth = 100;

    [Tooltip("Health bar visible to user")]
    [SerializeField]
    private RectTransform HealthBar;

    // do not show in IDE 
    public uint CurrentHealth { get; set; }


    public Health()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// reduce the health by the given amount
    /// </summary>
    public void TakeDamage(uint amount)
    {
        if (amount == 0)
            return;

        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("Dead");
            var controller = this.GetComponent<BasicObjectController>();
            if(controller != null)
                controller.OnZeroHealth();

        }
        // update the health bar to reflect Current Health (health remaining)
        HealthBar.sizeDelta = new Vector2(CurrentHealth, HealthBar.sizeDelta.y);
    }

    /// <summary>
    /// increase the current health by the given amount
    /// </summary>
    /// <param name="amount"></param>
    public void TakeHeal(uint amount)
    {
        // add to health, but keep max health threshold
        CurrentHealth = Math.Min(CurrentHealth += amount, MaxHealth);
    }

    /// <summary>
    /// change max health to new value
    /// </summary>
    public void UpdateMaxHealth(uint newMaxHealthAmount)
    {
        MaxHealth = newMaxHealthAmount;
    }
}