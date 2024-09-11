using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraController
{
    Transform vCamera;
    Transform lookAt;
    Transform rig;

    Vector3 currentLocalPosition = new Vector3(0, 1.43f, -4.15f);

    public float camRotationRadius = 3f;
    public Vector2 radiusLimit = new Vector3(1.5f, 6);
    public Vector2 verticalLimit = new Vector3(30, 120);

    public CameraController(Transform vCam, Transform rig, Transform look)
    {
        vCamera = vCam;
        this.rig = rig;
        lookAt = look;
    }

    public void HandleMouseInput(float h, float v)
    {
        if (h == 0 && v == 0) vCamera.position = rig.position + currentLocalPosition;
        var spherical = CoordConverter.CartesianToSpherical(vCamera.position - rig.position);

        spherical.x = Mathf.Clamp(camRotationRadius, radiusLimit.x, radiusLimit.y);
        spherical.y = (spherical.y * Mathf.Rad2Deg + h) * Mathf.Deg2Rad;
        spherical.z = Mathf.Clamp(spherical.z * Mathf.Rad2Deg - v, verticalLimit.x, verticalLimit.y) * Mathf.Deg2Rad;

        currentLocalPosition = CoordConverter.SphericalToCartesian(spherical);
        vCamera.position = rig.position + currentLocalPosition;
    }

    public void LimitCameraDistance()
    {
        var result = Physics.SphereCast(lookAt.position, 0.5f, (vCamera.position - lookAt.position).normalized, out var hit, camRotationRadius);

        Vector3 spherical;

        if (!result)
        {
            spherical = CoordConverter.CartesianToSpherical(vCamera.position - rig.position);

            spherical.x = radiusLimit.y;

            currentLocalPosition = CoordConverter.SphericalToCartesian(spherical);

            vCamera.position = rig.position + currentLocalPosition;

            return;
        }

        spherical = CoordConverter.CartesianToSpherical(vCamera.position - rig.position);

        spherical.x = Mathf.Clamp(hit.distance, radiusLimit.x, radiusLimit.y);

        currentLocalPosition = CoordConverter.SphericalToCartesian(spherical);

        vCamera.position = rig.position + currentLocalPosition;
    }

    public Vector3 GetCameraForward()
    {
        Vector3 forward = vCamera.forward;
        forward.y = 0;
        return forward.normalized;
    }

    public Quaternion GetCameraRotation()
    {
        Vector3 rot = vCamera.rotation.eulerAngles;
        rot.y = 0;
        return Quaternion.Euler(rot.normalized);
    }
}
