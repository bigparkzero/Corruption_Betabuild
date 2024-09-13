using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GimmickManager : Singleton<GimmickManager>
{
    #region 카메라 전환
    public CinemachineFreeLook freeLookCamera;
    private List<CinemachineVirtualCameraBase> allCameras = new List<CinemachineVirtualCameraBase>();
    private int activeCameraPriority = 10;
    private int inactiveCameraPriority = 0;

    private CinemachineBrain brain;
    #endregion
   
    void Start()
    {
        // 메인 카메라에 CinemachineBrain이 있어야 합니다.
        brain = Camera.main.GetComponent<CinemachineBrain>();

        // 씬의 모든 활성화된 카메라를 찾고 비활성화합니다.
        foreach (var cam in FindObjectsOfType<CinemachineVirtualCameraBase>())
        {
            if (cam.gameObject.activeSelf)
            {
                allCameras.Add(cam);
                cam.gameObject.SetActive(false);
            }
        }

        // 프리룩 카메라를 활성화합니다.
        if (freeLookCamera != null)
        {
            freeLookCamera.gameObject.SetActive(true);
        }
    }

    // 카메라를 전환하는 함수
    public void SwitchToCamera(CinemachineVirtualCameraBase newCamera)
    {

        // 프리룩 카메라를 비활성화합니다.
        if (freeLookCamera != null)
        {
            freeLookCamera.gameObject.SetActive(false);
        }

        // 모든 카메라의 우선순위를 낮추고 비활성화합니다.
        foreach (var cam in allCameras)
        {
            cam.gameObject.SetActive(false);
            cam.Priority = inactiveCameraPriority;
        }

        // 새로운 카메라를 활성화하고 우선순위를 높입니다.
        if (newCamera != null)
        {
            newCamera.gameObject.SetActive(true);
            newCamera.Priority = activeCameraPriority;
        }
    }

   
}



