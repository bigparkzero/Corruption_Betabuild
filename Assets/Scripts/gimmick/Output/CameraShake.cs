using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : GimmickOutput
{
    public Vector3 Velocity;
    public override void Act()
    {
        CinemachineImpulseSource Shaker = GetComponent<CinemachineImpulseSource>();
        Shaker.GenerateImpulse(Velocity);
        isDone = true;
    }
}
