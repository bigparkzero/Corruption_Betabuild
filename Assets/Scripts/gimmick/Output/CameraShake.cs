using Cinemachine;
using UnityEngine;

public class CameraShake : GimmickOutput
{
    
    public void CameraShakeing(float value)
    {
        CinemachineImpulseSource Shaker = GetComponent<CinemachineImpulseSource>();
        Shaker.GenerateImpulse(new Vector3(Random.Range(-value, value), Random.Range(-value, value), Random.Range(-value, value)));
        isDone = true;
    }
}
