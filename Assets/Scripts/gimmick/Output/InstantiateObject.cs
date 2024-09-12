
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObject : GimmickOutput
{
    public Transform parentPrefab;  // Canvas를 연결할 변수
    public GameObject prefab;

    public override void Act()
    {
        if (parentPrefab == null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity, parentPrefab);
        }

        // 프리팹을 생성하고, Canvas의 자식으로 설정
        GameObject instantiatedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, parentPrefab);

        // RectTransform이 있는지 확인하고, 필요한 설정을 진행
        RectTransform uiPrefab = instantiatedObject.GetComponent<RectTransform>();
        if (uiPrefab != null)
        {
            // RectTransform에 대한 설정 (예: 위치, 크기 조정 등)
            uiPrefab.anchoredPosition = Vector2.zero;  // UI 요소의 위치 설정
            uiPrefab.sizeDelta = new Vector2(100, 100); // UI 요소의 크기 설정
        }

        isDone = true;
    }
}