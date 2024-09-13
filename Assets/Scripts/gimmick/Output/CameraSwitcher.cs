using Cinemachine;
using UnityEngine;

public class CameraSwitcher : GimmickOutput
{
    public CinemachineVirtualCameraBase cam;

    public void Act()
    {
        GimmickManager.Instance.SwitchToCamera(cam);
        isDone = true;
    }
    public void ActResetCamera()
    {
        GimmickManager.Instance.SwitchToCamera(GimmickManager.Instance.freeLookCamera);
        isDone = true;
    }
}
