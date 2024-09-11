using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionController
{
    Transform character;

    public Vector3 FOOT_OFFSET = new Vector3(0, 0.08f, 0);
    public float CLIFF_CHECK_RADIUS = 0.2f;
    public float CLIFF_RAY_DISTANCE = 3f;

    public float STEP_RADIUS = 0.1f;
    public Vector3 SIDE_CHECK_OFFSET = new Vector3(0.03f, 0, 0);

    public Vector3 REPEAT_CHECK_OFFSET = new Vector3(0, 0.15f, 0);
    public int REPEAT_CHECK_COUNT = 6;

    LayerMask LAYERMASK_GROUND = LayerMask.GetMask("Ground");

    RaycastHit[] hits = new RaycastHit[10];

    public bool DetectCliff()
    {
        Vector3 checkPos = character.position + FOOT_OFFSET + character.forward * CLIFF_CHECK_RADIUS;

        if (Physics.RaycastNonAlloc(checkPos, Vector3.down, hits, CLIFF_RAY_DISTANCE, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0)
        {
            System.Array.Clear(hits, 0, hits.Length);
            return false;
        }
        return true;
    }

    public Vector3 GetCliffRightDirection()
    {
        Vector3 result = character.right;
        Vector3 checkPos = character.position - FOOT_OFFSET + character.forward * CLIFF_CHECK_RADIUS;

        if (Physics.RaycastNonAlloc(checkPos, -character.forward, hits, CLIFF_CHECK_RADIUS + 1f, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0)
        {
            RaycastHit hit = hits[0];
            result = Vector3.Cross(Vector3.up, hit.normal).normalized;
            System.Array.Clear(hits, 0, hits.Length);
        }

        return result;
    }

    public float CheckStepHeight()
    {
        float result = 0;
        Vector3 checkPos = character.position + FOOT_OFFSET;

        if (Physics.RaycastNonAlloc(checkPos, character.forward, hits, STEP_RADIUS, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0 ||
            Physics.RaycastNonAlloc(checkPos + SIDE_CHECK_OFFSET, character.forward, hits, STEP_RADIUS, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0 ||
            Physics.RaycastNonAlloc(checkPos - SIDE_CHECK_OFFSET, character.forward, hits, STEP_RADIUS, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0)
        {
            result = hits[0].point.y;
            for (int i = 0; i < REPEAT_CHECK_COUNT; i++)
            {
                System.Array.Clear(hits, 0, hits.Length);
                if (Physics.RaycastNonAlloc(checkPos + REPEAT_CHECK_OFFSET * (i + 1), character.forward, hits, STEP_RADIUS, LAYERMASK_GROUND, QueryTriggerInteraction.Ignore) > 0)
                {
                    if (i == REPEAT_CHECK_COUNT - 1) return 0;

                    result = hits[0].point.y;
                }
                else
                {
                    break;
                }
            }
        }

        return result;
    }
}
