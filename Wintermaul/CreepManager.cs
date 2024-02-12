using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreepManager : MonoBehaviour
{
    public static CreepManager instance;

    [SerializeField]
    Transform[] spawnPoints;

    [SerializeField]
    CreepScriptable[] creepList;

    [SerializeField]
    Transform destination;
    public Transform Destination => destination;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //SpawnCreeps();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) { SpawnCreeps(); } 
    }

    void SpawnCreeps()
    {
        foreach(Transform t in spawnPoints)
        {
            foreach(var point in t.GetComponentsInChildren<Transform>())
            {
                if (point == t) continue;
                GameObject newCreep = Instantiate<GameObject>(creepList[0].MeshPrefab, point.position, Quaternion.identity);
            }
        }
    }
}
