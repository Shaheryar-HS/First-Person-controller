using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // public float health = 100f;

    // public void TakeDamage(float damage)
    // {
    //     health -= damage;

    //     if (health <= 0)
    //     {
    //         Die();
    //     }
    // }
    public event Action<GameObject> OnEnemyDeath;

    public void Die()
    {
        OnEnemyDeath?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}
