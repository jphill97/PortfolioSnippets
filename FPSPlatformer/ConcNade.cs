using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

public class ConcNade : MonoBehaviour
{

    public bool isSticky = true;
    public Explosive explosive;

    public void Explode()
    {
        explosive.TriggerExplosion();
    }
}
