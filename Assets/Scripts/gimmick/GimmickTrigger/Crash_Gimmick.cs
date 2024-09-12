using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crash_Gimmick : GimmickTrigger
{
    public bool TriggerActionDone;
    public enum eDetectiontype
    {
        enter, // 콜라이더 박스와 충돌 했을때
        delay, // float만큼 기다리고 트리거
        exit // 콜라이더 박스와 충돌이 끝났을때
    }

    public eDetectiontype detectionType; // 트리거 유형

    public LayerMask DetectionLayer; // 중돌 감지 레이어

    public float delayTime;

    private float timer; 


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            if (detectionType == eDetectiontype.enter)
            {
                Debug.Log("enter충돌 감지함");
                InvokeEventRunOnTrigger();
            }
            timer = 0;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0 && detectionType == eDetectiontype.delay)
        {
            if (timer >= delayTime)
            {
                Debug.Log(delayTime + " 초 만큼 기다린 후 감지함");
                InvokeEventRunOnTrigger();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            if (detectionType == eDetectiontype.exit)
            { 
                Debug.Log("exit충돌 감지함");
                InvokeEventRunOnTrigger();
            }
            timer = 0;
        }
    }
}
