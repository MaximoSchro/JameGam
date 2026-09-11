using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]  private int maxHealth;
    private int _currentHealth;

    private void Start()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        _currentHealth = maxHealth;
    }

    internal void TakeDamage(int damage = 1)
    {
        _currentHealth -= damage;

        if (damage <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        //change this bullshit :P
        Destroy(gameObject);
    }
    public void TimedDeath(int time)
    {
        Destroy(gameObject,time);
    }
}
