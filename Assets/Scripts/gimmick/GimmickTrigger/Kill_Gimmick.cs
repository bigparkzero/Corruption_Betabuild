using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Kill_Gimmick : GimmickTrigger
{
    public List<GameObject> killDetectionObjects;
    private bool a;
    private void Start()
    {
        a = false;
    }
    private void Update()
    {
        bool allInactive = true;

        // 리스트의 모든 객체를 순회하면서 활성 상태를 확인
        for (int i = 0; i < killDetectionObjects.Count; i++)
        {
            if (killDetectionObjects[i] == null)
            {
                continue;
            }
            else if (killDetectionObjects[i].activeSelf)
            {
                allInactive = false; // 하나라도 활성화된 객체가 있으면 false로 설정
                break; // 더 이상 순회할 필요가 없으므로 루프 종료
            }
        }

        // 모든 객체가 비활성화된 경우에만 함수 호출
        if (allInactive && killDetectionObjects != null && !a)
        {
            InvokeEventRunOnTrigger();
            a = true;
        }
    }
}
