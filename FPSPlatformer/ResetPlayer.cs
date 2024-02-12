using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    public Transform resetTrans;

    public void SetResetPos()
    {
        ResetTriggerManager.instance.SetResetPosition(resetTrans.position);
    }
}
