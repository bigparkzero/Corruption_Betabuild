using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayer : GimmickOutput
{

    public PlayerMainController controller;

    public bool isFreeze;

    public void Act()
    {
        if (isFreeze)
        {
            controller.enabled = false;
        }
        else 
        {
            controller.enabled = true;
        }
        isDone = true;
    }
}
