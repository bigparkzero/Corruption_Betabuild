using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Gimmick : GimmickTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        InvokeEventRunOnTrigger();
    }
}
