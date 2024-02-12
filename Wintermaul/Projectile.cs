using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Unit target;
    bool init = false;
    bool useSplash = false;
    float splashRadius = 0f;
    float splashPercentage = 0f;
    float moveSpeed = 0f;
    float damage = 0f;
    Vector3 targetPos = Vector3.zero;


    public void Init(Unit Target, float MoveSpeed = 10f, float Damage = 1f, bool UseSplash = false, float SplashRadius = 0f, float SplashPercentage = 0f)
    {
        target = Target;
        moveSpeed = MoveSpeed;
        damage = Damage;
        useSplash = UseSplash;
        splashPercentage = SplashPercentage;
        splashRadius = SplashRadius;
        init = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(init)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        if(target != null) 
        { 
            targetPos = target.transform.position + Vector3.up; 
        }
        else
        {
            if(transform.position == targetPos) 
            {
                Debug.Log("Hit dead creep!");
                Destroy(gameObject);
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
    }

    void CheckSplashDamage(Collider hitCol)
    {
        Collider[] hitColls = Physics.OverlapSphere(transform.position, splashRadius, 1 << 6);
        foreach (Collider hit in hitColls)
        {
            if (hit.tag == "Creep" && hitCol != hit)
            {
                hit.GetComponentInParent<Creep>().TakeDamage(damage * (splashPercentage/100f));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(target != null)
        {
            if (other.transform.parent == target.transform)
            {
                Debug.Log("Hit!");

                if (useSplash)
                {
                    CheckSplashDamage(other);
                }

                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
