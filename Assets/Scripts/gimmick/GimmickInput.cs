using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GimmickInput : MonoBehaviour
{
    public bool testbool;
    [Header("자식오브젝트에서 gimmick trigger를 가진 오브젝트를 탐색\n별도로 트리거를 넣어서 다중 트리거를 구현할 수 있습니다")]
    public List<GimmickTrigger> triggers;
    
    public enum typetotrigger
    {
        And,
        Or,
        Sequentially
    }

    [Header("And = 전부 트리거 되야함\nOr = 트리거중 하나라도 트리거되면 실행 \nSequentially = 리스트의 순서대로 트리거 되야함")]
    public typetotrigger TypeToTrigger;
    private int sequentialIndex = 0;
    bool CheckAllTriggers()
    {
        foreach (var trigger in triggers)
        {
            if (!trigger.isTriggered) // 하나라도 트리거되지 않았다면 false 반환
            {
                return false;
            }
        }
        return true; // 모두 트리거되었다면 true 반환
    }

    // 트리거 중 하나라도 발동되었는지 확인하는 함수 (Or 모드)
    bool CheckAnyTrigger()
    {
        foreach (var trigger in triggers)
        {
            if (trigger.isTriggered) // 하나라도 트리거되었다면 true 반환
            {
                return true;
            }
        }
        return false; // 아무 트리거도 발동되지 않았으면 false 반환
    }

    // 순차적으로 트리거가 발동되었는지 확인하는 함수 (Sequentially 모드)
    bool CheckSequentialTrigger()
    {
        // 현재 순서의 트리거가 발동되었는지 확인
        if (sequentialIndex < triggers.Count && triggers[sequentialIndex].isTriggered)
        {
            sequentialIndex++; // 다음 순서로 이동

            // 모든 트리거가 순차적으로 발동되었으면 true 반환
            if (sequentialIndex >= triggers.Count)
            {
                sequentialIndex = 0; // 순서 초기화 (다음 체크를 위해)
                return true;
            }
        }

        return false;
    }
    public void TriggerCheck()
    {
        if (triggers.Count != 0)
        {
            if (triggers.Count <= 2)
            {
                switch (TypeToTrigger)
                {
                    case typetotrigger.And:
                        if (CheckAllTriggers())
                        {
                            InvokeEvent();
                        }
                        break;

                    case typetotrigger.Or:
                        if (CheckAnyTrigger())
                        {
                            InvokeEvent();
                        }
                        break;

                    case typetotrigger.Sequentially:
                        if (CheckSequentialTrigger())
                        {
                            InvokeEvent();
                        }
                        break;
                }
            }
            else
            {
                InvokeEvent();
            }
        }
    }



[System.Serializable]
    public class SimpleEvent : UnityEvent
    {
    }

    public List<SimpleEvent> OutputEvent;

    public bool triggerMultipleTimes;

    public void InvokeEvent()
    {
        if (triggerMultipleTimes)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                triggers[i].isTriggered = false;
            }
            for (int i = 0; i < OutputEvent.Count; i++)
            {
                for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
                {
                    Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                    GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                    if (associatedOutput != null)
                    {
                        associatedOutput.isDone = false;
                    }
                }
            }
        }
        if (OutputEvent != null && OutputEvent.Count > 0)
        {
            StartCoroutine(InvokeEventsCoroutine());
        }
    }

    private bool isOutputGimmick;
    private bool isDoneAll;


    private IEnumerator InvokeEventsCoroutine()
    {
        OutputEvent[0].Invoke();

        // OutputEvent 리스트를 순회하면서 하나씩 실행
        for (int i = 0; i < OutputEvent.Count; i++)
        {
            isOutputGimmick = false;
            isDoneAll = false; // 매 이벤트마다 초기화

            for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
            {
                Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                if (associatedOutput != null)
                {
                    isOutputGimmick = true;
                }
            }

            if (isOutputGimmick)
            {
                // 해당 오브젝트의 isDone이 true가 될 때까지 대기
                while (!isDoneAll)
                {
                    yield return null;
                    isDoneAll = true; // 기본적으로 완료된 것으로 설정
                    for (int j = 0; j < OutputEvent[i].GetPersistentEventCount(); j++)
                    {
                        Object targetObj = OutputEvent[i].GetPersistentTarget(j);
                        GimmickOutput associatedOutput = targetObj.GetComponent<GimmickOutput>();
                        if (associatedOutput != null && !associatedOutput.isDone)
                        {
                            isDoneAll = false; // 완료되지 않은 경우 false로 설정
                            break; // 더 이상 확인할 필요 없음
                        }
                    }

                    print("기다리는 중");
                }
            }

            // i + 1이 OutputEvent.Count를 초과하지 않도록 확인
            if (i + 1 < OutputEvent.Count)
            {
                OutputEvent[i + 1].Invoke();
                print(i + "번째 이벤트 실행");
            }
        }
    }

}
