using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    int currentHealth;
    public int maxHealth;
    public event Action<Actor> OnDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if(currentHealth <= 0)
        { Death(); }
    }
    
    protected virtual void Death()
    {
        // Death function
        // TEMPORARY: Destroy Object
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
