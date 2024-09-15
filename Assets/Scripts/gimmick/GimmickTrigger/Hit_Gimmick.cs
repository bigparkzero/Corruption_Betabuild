using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Gimmick : GimmickTrigger
{
    public LayerMask defaultLayerMask;
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 디폴트 레이어에 있는지 확인
        if ((defaultLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            // 디폴트 레이어에 속하는 객체일 경우에만 이벤트 실행
            InvokeEventRunOnTrigger();
        }
    }
}
