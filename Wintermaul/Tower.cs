using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Tower : Unit
{
    [SerializeField]
    GameObject meshObj;

    [SerializeField]
    Transform firepoint;

    [SerializeField]
    GameObject colliderObject;

    [SerializeField]
    Projectile projectile;

    TowerScriptable towerStats;
    bool active = false;
    Unit target;
    float attackTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active)
        {
            if (target == null)
            {
                target = CheckForTarget();
            }
            
            if(target != null)
            {
                TryAttack();
            }
        }
    }

    void TryAttack()
    {

        if (Vector3.Distance(transform.position, target.transform.position) > towerStats.AttackRange + 1)
        {
            target = null;
            return;
        }


        if (Time.time < attackTime)
            return;
        

        //Divide 60 sec by attack per min to get seconds between attacks
        attackTime = Time.time + 60f/towerStats.AttackSpeed;
        Projectile newProj = Instantiate<GameObject>(projectile.gameObject).GetComponent<Projectile>();
        newProj.transform.position = firepoint.position;

        newProj.Init(target,
            towerStats.ProjectileSpeed,
            towerStats.BaseDamage,
            towerStats.SplashRadius > 0 ? true : false,
            towerStats.SplashRadius,
            towerStats.SplashPercentage);
    }

    Unit CheckForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up, towerStats.AttackRange + 1,1<<6);
        Unit newTarget = null;
        float dist = towerStats.AttackRange*2f;

        foreach(Collider hit in hits)
        {
            if(hit.tag == "Creep")
            {
                float newDist = Vector3.Distance(hit.ClosestPoint(transform.position), transform.position);
                if(newDist < dist)
                {
                    newTarget = hit.gameObject.GetComponentInParent<Unit>();
                    dist = newDist;
                }
            }
        }

        return newTarget;
    }

    public void StartConstruction(TowerScriptable ts)
    {
        UnitInfo = ts;
        towerStats = UnitInfo as TowerScriptable;
        colliderObject.SetActive(true);
        GetUnitStats();
        StartCoroutine(BuildSelf());
    }

    IEnumerator BuildSelf()
    {
        meshObj.transform.localScale = Vector3.one * .25f;
        yield return new WaitForSeconds(towerStats.ConstructionTime/3f);
        meshObj.transform.localScale = Vector3.one * .5f;
        yield return new WaitForSeconds(towerStats.ConstructionTime/3f);
        meshObj.transform.localScale = Vector3.one * .75f;
        yield return new WaitForSeconds(towerStats.ConstructionTime/3f);
        meshObj.transform.localScale = Vector3.one;
        active = true;
    }
}
