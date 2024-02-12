using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public class ResetTriggerManager : MonoBehaviour
{
    public static ResetTriggerManager instance;

    public Transform playerTrans;
    public Vector3 resetPos;
    public bool preserveVelocity;

    CharacterController cc;
    PlayerCharacterController pcc;

    private void Awake()
    {
        instance = this;
    }

    public void SetResetPosition(Vector3 pos)
    {
        resetPos  = pos;
    }
    
    public void SetPlayerPosition()
    {
        Debug.Log("Reset Player");

        if(cc == null)
            cc = playerTrans.GetComponent<CharacterController>();

        if (pcc == null)
            pcc = playerTrans.GetComponent<Unity.FPS.Gameplay.PlayerCharacterController>();

        cc.enabled = false;
        playerTrans.position = resetPos;
        cc.enabled = true;

        if (!preserveVelocity)
        {
           pcc.CharacterVelocity = Vector3.zero;
        }
    }
}
