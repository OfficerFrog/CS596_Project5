using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    // TODO: allow PlayerController to change max health (e.g. increase in level)
    public int MaxHealth = 100;
    
    // do not show in IDE 
    public int CurrentHealth { get; set; }

    public Health()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// reduce the health of the user
    /// </summary>
    public void TakeDamage(int amount)
    {
        // negative health (adding to health) should call TakeHeal() method
        if (amount <= 0)
            return;

        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("Dead");
        }
    }

    public void TakeHeal(int amount)
    {
        // add to health, but keep max health threshold
        CurrentHealth = Math.Min(CurrentHealth += amount, MaxHealth);
    }
}