using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivate_Gimmick : GimmickTrigger
{
    private void OnDisable()
    {
        InvokeEventRunOnTrigger();
    }
}
