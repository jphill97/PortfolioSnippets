using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    protected UnitScriptable UnitInfo;



    protected float currentHealth;
    protected float currentArmor;
    protected float currentMovespeed;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        if(UnitInfo != null)
        {
            RegenHealth();
        }
    }

    protected virtual void RegenHealth()
    {
        if (currentHealth == UnitInfo.Health)
            return;

        currentHealth += UnitInfo.HealthRegen * Time.fixedDeltaTime;

        if(currentHealth > UnitInfo.Health)
            currentHealth = UnitInfo.Health;
    }

    protected virtual void GetUnitStats()
    {
        currentHealth = UnitInfo.Health;
        currentArmor = UnitInfo.Armor;
        currentMovespeed = UnitInfo.MovementSpeed;
    }

    public virtual bool TakeDamage(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
