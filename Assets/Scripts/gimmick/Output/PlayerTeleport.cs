using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : GimmickOutput
{
    public PlayerMainController controller;
    public Transform PlayerTeleportPosition;
    public override void Act()
    {
        controller.transform.position = PlayerTeleportPosition.position;
        isDone = true;
    }

   
}
