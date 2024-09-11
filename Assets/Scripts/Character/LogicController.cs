using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LogicController
{
    public Vector3 logicalForward
    {
        get;
        private set;
    }
    MainController main;
    CharacterController controller;

    public float aimingDistance = 4f;
    public float aimingAngle = 45f;

    float OBSTACLE_DETECTION_DISTANCE = 1.5f;

    LayerMask enemyLayer = LayerMask.GetMask("Enemy");
    LayerMask groundLayer = LayerMask.GetMask("Ground");

    //TODO: buff, debuff, ailments

    public LogicController(Vector3 forward, MainController main, CharacterController controller)
    {
        logicalForward = forward;
        this.main = main;
        this.controller = controller;
    }

    public void SetLogicalForward(Quaternion targetRotation)
    {
        logicalForward = targetRotation * Vector3.forward;
    }

    public GameObject GetAimingTarget()
    {
        Vector3 playerCenter = main.transform.position + Vector3.up;
        Collider[] nearEnemies = Physics.OverlapSphere(playerCenter, aimingDistance, enemyLayer, QueryTriggerInteraction.Ignore);

        List<Transform> detectedEnemies = new List<Transform>();
        foreach (Collider collider in nearEnemies)
        {
            Vector3 enemyCenterPos = collider.transform.position + Vector3.up;
            float angle = Vector3.Angle(logicalForward, enemyCenterPos - playerCenter);

            if (angle < aimingAngle)
            {
                detectedEnemies.Add(collider.transform);
            }
        }

        float closestDistance = float.MaxValue;
        GameObject closest = null;
        foreach (Transform transform in detectedEnemies)
        {
            float dist = Vector3.Distance(main.transform.position, transform.position);

            if (dist < closestDistance)
            {
                closest = transform.gameObject;
                closestDistance = dist;
            }
        }

        return closest;
    }

    public Collider DetectObstacle(Vector3 camBasedDirection)
    {
        if (camBasedDirection == Vector3.zero) return null;

        Bounds bounds = controller.bounds;
        float kneeHeight = bounds.center.y - bounds.min.y;
        Vector3 knee = bounds.center - Vector3.up * kneeHeight;

        RaycastHit[] hits = new RaycastHit[1];

        if (Physics.RaycastNonAlloc(knee, logicalForward, hits, OBSTACLE_DETECTION_DISTANCE, groundLayer, QueryTriggerInteraction.Ignore) > 0)
        {
            return hits[0].collider;
        }

        return null;
    }
}
