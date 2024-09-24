using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathIndicator : MonoBehaviour
{
    public Transform currentPosition;
    public LineRenderer lineRenderer;
    public NavMeshAgent agent;
    public Transform target; // ������
    public int resolution = 10; // ��� ������ ����
    public float fixedY = 0.1f; // ������ �ٴڿ� "������" Y ��ǥ
    public LayerMask raymask;

    void Start()
    {
        // ������ ī�޶� ������ ���� �ʵ��� ����
        lineRenderer.alignment = LineAlignment.TransformZ;
        //NavMesh.pathfindingIterationsPerFrame = 1000;
    }

    void Update()
    {
        agent.gameObject.transform.position = target.position;
        // �������� �̵�
        agent.SetDestination(currentPosition.position);
        // ��θ� �ٴڿ� �׸���
        DrawCurvedPath();
    }

    void DrawCurvedPath()
    {
        NavMeshPath path = agent.path;
        List<Vector3> smoothPathPoints = new List<Vector3>();

        if (path.corners.Length < 2)
            return;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 p0 = i == 0 ? path.corners[i] : path.corners[i - 1];
            Vector3 p1 = path.corners[i];
            Vector3 p2 = path.corners[i + 1];
            Vector3 p3 = i == path.corners.Length - 2 ? path.corners[i + 1] : path.corners[i + 2];

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 point = CatmullRom(p0, p1, p2, p3, t);

                // Raycast�� ������ Y ��ǥ�� ���
                RaycastHit hit;
                if (Physics.Raycast(point + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, raymask))
                {
                    point.y = hit.point.y + fixedY; // ���� ���� 0.1 ������ ����
                }

                smoothPathPoints.Add(point);
            }
        }

        lineRenderer.positionCount = smoothPathPoints.Count;
        for (int i = 0; i < smoothPathPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, smoothPathPoints[i]);
        }
    }


    public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }
}
