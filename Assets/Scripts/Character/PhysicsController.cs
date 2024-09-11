using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicsController
{
    Transform character;
    CharacterController controller;

    float currentVerticalVelocity;
    public Quaternion targetRotation;

    public float TURN_RATE = 10f;
    public float GRAVITY = -43f;
    public float FALLING_ACCELERATION_RATE = 1.2f;

    const float COLLIDE_CHECK_RADIUS = 0.24f;
    const float COLLIDE_CHECK_OFFSET = 0.1f;
    LayerMask GROUND_MASK = LayerMask.GetMask("Ground");
    LayerMask RAGDOLL_MASK = LayerMask.GetMask("Ragdoll");
    const float NEAR_GROUND_FALL_TIME = 0.1f;
    bool isGrounded;

    public PhysicsController(Transform character, CharacterController controller)
    {
        this.character = character;
        this.controller = controller;
    }

    public Vector3 GetCenter()
    {
        return controller.center;
    }

    public bool IsFallingDown()
    {
        return !isGrounded && currentVerticalVelocity <= 0;
    }

    public bool IsHeadColliding()
    {
        Collider[] cols = Physics.OverlapSphere(character.position + Vector3.up * (controller.bounds.max.y - COLLIDE_CHECK_OFFSET), COLLIDE_CHECK_RADIUS, GROUND_MASK, QueryTriggerInteraction.Ignore);

        return cols.Length > 0;
    }

    public bool IsGrounded()
    {
        Collider[] cols = Physics.OverlapSphere(character.position + Vector3.up * COLLIDE_CHECK_OFFSET, COLLIDE_CHECK_RADIUS, GROUND_MASK | RAGDOLL_MASK, QueryTriggerInteraction.Ignore);

        isGrounded = cols.Length > 0;
        return isGrounded;
    }

    public bool IsNearGround()
    {
        float nearGroundDIstance = -currentVerticalVelocity * NEAR_GROUND_FALL_TIME + 0.5f * GRAVITY * NEAR_GROUND_FALL_TIME * NEAR_GROUND_FALL_TIME;
        if (Physics.Raycast(character.position, Vector3.down, nearGroundDIstance, GROUND_MASK | RAGDOLL_MASK, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public Collider IsWallCollided(Vector3 camBasedDirection)
    {
        if (camBasedDirection == Vector3.zero) return null;

        Collider[] walls = new Collider[8];
        Bounds bounds = controller.bounds;
        Vector3 center = bounds.center;
        float radius = COLLIDE_CHECK_OFFSET + (bounds.max.x - bounds.min.x) / 2;
        //if (Physics.OverlapSphereNonAlloc(bounds.center, radius, walls, GROUND_MASK, QueryTriggerInteraction.Ignore) > 0)
        //{
        //    float closest = float.MaxValue;
        //    Collider result = null;
        //    foreach (Collider col in walls)
        //    {
        //        Vector3 closestPoint = col.ClosestPoint(bounds.center);
        //
        //        Vector3 dir = closestPoint - center;
        //        if (Vector3.Angle(dir, camBasedDirection) > 45) continue;
        //
        //        float closestDistance = Vector3.Distance(closestPoint, center);
        //
        //        if (closest > closestDistance)
        //        {
        //            closest = closestDistance;
        //            result = col;
        //        }
        //
        //        if (result != null) return result;
        //    }
        //}
        //return null;
        int hitCount = Physics.OverlapSphereNonAlloc(bounds.center, radius, walls, GROUND_MASK, QueryTriggerInteraction.Ignore);

        if (hitCount > 0)
        {
            float closest = float.MaxValue;
            Collider result = null;

            // hitCount만큼의 collider만 체크
            for (int i = 0; i < hitCount; i++)
            {
                Collider col = walls[i];
                if (col == null) continue;  // null 체크

                Vector3 closestPoint = col.ClosestPoint(bounds.center);
                Vector3 dir = closestPoint - center;

                // 방향 비교
                if (Vector3.Angle(dir, camBasedDirection) > 45) continue;

                float closestDistance = Vector3.Distance(closestPoint, center);

                // 가장 가까운 충돌체를 탐색
                if (closest > closestDistance)
                {
                    closest = closestDistance;
                    result = col;
                }
            }

            return result;  // 가장 가까운 충돌체 반환
        }

        return null;  // 충돌 없음
    }

    public void MoveHorizontal(Vector3 dir, float power)
    {
        if (dir == Vector3.zero || power == 0) return;
        dir.y = 0;
        controller.Move(dir.normalized * power * Time.deltaTime);
    }

    public void MoveVertical()
    {
        controller.Move(Vector3.up * currentVerticalVelocity * Time.deltaTime);
    }

    public void MoveVerticalForced(Vector3 dir, float power)
    {
        dir.x = 0;
        dir.z = 0;
        controller.Move(dir.normalized * power * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if (IsGrounded()) currentVerticalVelocity = -2;
        else currentVerticalVelocity += GRAVITY * Time.deltaTime;
    }

    public void SetVerticalVelocity(float jumpAmount)
    {
        currentVerticalVelocity = Mathf.Sqrt(-2 * GRAVITY * jumpAmount);
        isGrounded = false;
    }

    public void SetTargetRotation(Quaternion targetRotation)
    {
        this.targetRotation = targetRotation;
    }

    public void Turn()
    {
        if (Quaternion.Angle(targetRotation, character.rotation) < 1) return;
        character.rotation = Quaternion.Slerp(character.rotation, targetRotation, TURN_RATE * Time.deltaTime);
    }

    public void TurnImmediately()
    {
        character.rotation = targetRotation;
    }
}
