using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creep : Unit
{
    NavMeshAgent agent;
    [SerializeField]
    HealthBar healthBar;
    // Start is called before the first frame update
    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        SetDestination();
        GetUnitStats();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void GetUnitStats()
    {
        base.GetUnitStats();
        healthBar.UpdateHealth(1);
        agent.speed = UnitInfo.MovementSpeed;
    }

    public override bool TakeDamage(float damage)
    {
        Debug.Log("update hp");
        healthBar.UpdateHealth(currentHealth - damage > 0 ? (currentHealth - damage)/UnitInfo.Health : 0);
        return base.TakeDamage(damage);

    }

    void SetDestination()
    {
        agent.SetDestination(CreepManager.instance.Destination.position);
    }
}
