using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event Action OnDeath;

    //we add virtual in order the function that will inherit this class will NOT override the method
    protected virtual void Start()
    {
        health = startingHealth;
    }
    public void TakeHit(float damage, RaycastHit hit)
    {
        //We will do some stuff here with the hit
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    //if we right click on the script and select Self destruct this function will executed
    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject);
    }


}
