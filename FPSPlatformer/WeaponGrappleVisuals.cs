using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class WeaponGrappleVisuals : MonoBehaviour
{
    public LineRenderer lr;

    public Vector3 grapplePos;

    public Unity.FPS.Game.WeaponController WC;

    List<Vector3> hitPositions = new List<Vector3>();
    List<Vector3> tempHits = new List<Vector3>();
    Transform muzzle;
    Vector3 endPoint;
    Vector3 hitPoint = Vector3.zero;
    bool recoiling = false;

    private void Start()
    {
        WC.grappleHit += GrappleHit;
        WC.grappleRelease += GrappleRelease;
    }

    // Update is called once per frame
    void Update()
    {
        if (!recoiling && lr.enabled)
        {
            UpdateGrapple();
        }
    }
    
    void UpdateGrapple()
    {
        tempHits.Clear();
        tempHits = new List<Vector3>(hitPositions);
        for(int i = hitPositions.Count - 1; i >= 0; i--)
        {
            RaycastHit hit2;
            RaycastHit hit;

            if(i - 1 >= 0 && Physics.Raycast(WC.Owner.transform.position, (hitPositions[i - 1] - WC.Owner.transform.position).normalized, out hit2) && Vector3.Distance(hit2.point, hitPositions[i - 1]) < .01f && hit2.collider.tag == "Grapple")
            {
                tempHits.RemoveAt(i);
                continue; 
            }
            else if (Physics.Raycast(WC.Owner.transform.position, (hitPositions[i] - WC.Owner.transform.position).normalized, out hit) && hit.collider.tag == "Grapple")
            {
                if (hit.point == hitPositions[i])
                {
                    break;
                }
                else
                {
                    tempHits.Add(hit.point);
                    break;
                }
            }
            
        }


        hitPositions = new List<Vector3>(tempHits);
        if (Vector3.Distance(hitPositions[hitPositions.Count - 1], hitPoint) > 0.01f)
        {
            hitPoint = hitPositions[hitPositions.Count - 1];
            WC.GrappleHitWall(hitPoint);
        }
            
        
        lr.positionCount = hitPositions.Count + 1;
        lr.SetPosition(hitPositions.Count, muzzle.position);
        for (int i = hitPositions.Count - 1; i >= 0; i--)
        {
            lr.SetPosition(i, hitPositions[i]);
        }
    }

    void GrappleRelease()
    {
        if (lr.enabled)
        {
            StartCoroutine(RecoilGrapple());
        }
    }

    void GrappleHit(Vector3 HitPoint, Transform Muzzle)
    {
        hitPositions.Clear();
        hitPositions.Add(HitPoint);
        endPoint = HitPoint;
        hitPoint = endPoint;
        muzzle = Muzzle;
        lr.enabled = true;
    }

    IEnumerator RecoilGrapple()
    {
        float timer = WC.AmmoReloadDelay;
        recoiling = true;
        lr.positionCount = 2;
        float dist = Vector3.Distance(hitPoint, muzzle.position);

        while (timer > 0)
        {
            hitPoint = Vector3.MoveTowards(hitPoint, muzzle.position, (dist * 2f * Time.deltaTime) / WC.AmmoReloadDelay);
            lr.SetPosition(0, muzzle.position);
            lr.SetPosition(1, hitPoint);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }

        lr.enabled = false;
        recoiling = false;
    }
}
