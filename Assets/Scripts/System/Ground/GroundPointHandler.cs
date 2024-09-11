using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class GroundPointHandler : MonoBehaviour
{
    [SerializeField] GameObject prefab_GroundPoint;

    [SerializeField] GroundPoint.Type pointType;

    float pointInterval = 0.4f; // 포인트 간격
    float inset = 0.1f; // 경계로부터 안쪽으로 들어갈 거리
    float characterHeight = 2.0f; // 캐릭터의 키
    float characterWidth = 1.0f; // 캐릭터의 두께

    [SerializeField] bool createPoints;
    [SerializeField] bool deleteAll;
    [SerializeField] List<GroundPoint> points = new List<GroundPoint>();

    public List<Vector3> test_ray_starts = new List<Vector3>();
    public List<Vector3> test_ray_ends = new List<Vector3>();

    Vector3 topLeft;
    Vector3 topRight;
    Vector3 bottomLeft;
    Vector3 bottomRight;

    public List<Vector3> hitList = new();


    void Update()
    {
        if (deleteAll)
        {
            deleteAll = false;
            DeleteAll();

            test_ray_starts.Clear();
            test_ray_ends.Clear();
            hitList.Clear();
        }

        if (createPoints)
        {
            createPoints = false;
            CreatePoints();
        }
    }

    public GroundPoint GetClosestGroundPoint(Vector3 characterPosition)
    {
        GroundPoint closestPoint = null;
        float minDistance = float.MaxValue;

        foreach (GroundPoint point in points)
        {
            if (point == null)
                continue;

            float distance = Vector3.Distance(characterPosition, point.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    void DeleteAll()
    {
        for (int i = points.Count - 1; i >= 0; --i)
        {
            if (points[i] == null) continue;
            
            DestroyImmediate(points[i].gameObject);
        }
        points.Clear();
    }

    void CreatePoints()
    {
        BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
        foreach (var collider in colliders)
        {
            CreatePointsOnTopFace(collider);
        }
    }

    void CreatePointsOnTopFace(BoxCollider collider)
    {
        Vector3 size = collider.size;
        Vector3 center = collider.center;

        topLeft = collider.transform.TransformPoint(center + new Vector3(-size.x / 2 + inset, size.y / 2, size.z / 2 - inset));
        topRight = collider.transform.TransformPoint(center + new Vector3(size.x / 2 - inset, size.y / 2, size.z / 2 - inset));
        bottomLeft = collider.transform.TransformPoint(center + new Vector3(-size.x / 2 + inset, size.y / 2, -size.z / 2 + inset));
        bottomRight = collider.transform.TransformPoint(center + new Vector3(size.x / 2 - inset, size.y / 2, -size.z / 2 + inset));

        PlacePointsOnVisibleArea(collider, topLeft, topRight, bottomRight, bottomLeft);
    }

    void PlacePointsOnVisibleArea(BoxCollider collider, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft)
    {
        Vector3[] corners = { topLeft, topRight, bottomRight, bottomLeft };
        Vector3[] directions = { (topRight - topLeft).normalized, (bottomRight - topRight).normalized, (bottomLeft - bottomRight).normalized, (topLeft - bottomLeft).normalized };

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[(i + 1) % corners.Length];
            Vector3 direction = directions[i];
            float distance = Vector3.Distance(start, end);

            int pointCount = Mathf.FloorToInt(distance / pointInterval);

            for (int j = 0; j <= pointCount; j++)
            {
                Vector3 position = start + direction * (j * pointInterval);

                // Check for visibility and space using Raycast
                if (IsPositionVisibleAndSpacious(collider, position))
                {
                    GameObject p = Instantiate(prefab_GroundPoint, position, Quaternion.identity);
                    p.transform.parent = transform;
                    GroundPoint groundPoint = p.GetComponent<GroundPoint>();
                    groundPoint.type = pointType;
                    points.Add(groundPoint);
                }
            }
        }
    }

    bool IsPositionVisibleAndSpacious(BoxCollider collider, Vector3 position)
    {
        Vector3 upward = Vector3.up;
        Vector3 downward = Vector3.down;

        // Check if there is space for the character above this point
        RaycastHit hit;

        // Raycast downward to check if this point is on the top surface
        if (Physics.Raycast(position + upward * 0.1f, downward, out hit, 1.0f))
        {
            test_ray_starts.Add(position + upward * 0.1f);
            test_ray_ends.Add(position + upward * 0.1f + downward);
            hitList.Add(hit.point);

            if (hit.collider != collider)
                return false; // The point is covered by another collider
        }

        // Additional checks for space
        Vector3 offset1 = new Vector3(characterWidth / 2, 0, 0);
        Vector3 offset2 = new Vector3(0, 0, characterWidth / 2);

        // Check space in the surrounding area (e.g., to ensure capsule can stand)
        if (!Physics.Raycast(position + upward * characterHeight, downward, out hit, characterHeight)
            && !Physics.Raycast(position + offset1 + upward * characterHeight, downward, out hit, characterHeight)
            && !Physics.Raycast(position - offset1 + upward * characterHeight, downward, out hit, characterHeight)
            && !Physics.Raycast(position + offset2 + upward * characterHeight, downward, out hit, characterHeight)
            && !Physics.Raycast(position - offset2 + upward * characterHeight, downward, out hit, characterHeight))
        {
            test_ray_starts.Add(position + upward * characterHeight);
            test_ray_starts.Add(position + offset1 + upward * characterHeight);
            test_ray_starts.Add(position - offset1 + upward * characterHeight);
            test_ray_starts.Add(position + offset2 + upward * characterHeight);
            test_ray_starts.Add(position - offset2 + upward * characterHeight);

            test_ray_ends.Add(position + upward * characterHeight + downward * characterHeight);
            test_ray_ends.Add(position + offset1 + upward * characterHeight + downward * characterHeight);
            test_ray_ends.Add(position - offset1 + upward * characterHeight + downward * characterHeight);
            test_ray_ends.Add(position + offset2 + upward * characterHeight + downward * characterHeight);
            test_ray_ends.Add(position - offset2 + upward * characterHeight + downward * characterHeight);

            hitList.Add(hit.point);

            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (hitList.Count <= 0) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(test_ray_starts[0], test_ray_ends[0]);
        Gizmos.DrawWireSphere(topLeft, 0.4f);
        Gizmos.DrawWireSphere(topRight, 0.4f);
        Gizmos.DrawWireSphere(bottomLeft, 0.4f);
        Gizmos.DrawWireSphere(bottomRight, 0.4f);

        Gizmos.color = Color.blue;
        for (int i = 0; i < test_ray_starts.Count; i++)
        {
            Vector3 start = test_ray_starts[i];
            Vector3 end = test_ray_ends[i];

            Gizmos.DrawRay(start, end - start);
        }

        Gizmos.color = Color.yellow;
        foreach (var hit in hitList)
        {
            Gizmos.DrawSphere(hit, 0.2f);
        }
    }
}
