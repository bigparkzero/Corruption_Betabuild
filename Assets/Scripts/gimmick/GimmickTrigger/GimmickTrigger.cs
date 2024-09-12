using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class GimmickTrigger : MonoBehaviour
{
    public void test(string text)
    {
        Debug.Log(text);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
    public void DestroyObject(GameObject target)
    {
        Destroy(target);
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








    public bool testbool;
    [HideInInspector] public bool isTriggered;
    [Header("GimmickInput을 설정할 수 있습니다.\n 설정값이 없을 경우 부모 오브젝트의 GimmickInput를 실행합니다")]
    public GimmickInput GimmickInput;
    private void Start()
    {
        isTriggered = false;
    }
    public void InvokeEventRunOnTrigger()
    {
        if (GimmickInput != null)
        {
            isTriggered = true;
            InvokeEvent();
            GimmickInput.TriggerCheck();
        }
        else
        {
            if (OutputEvent != null)
            {
                InvokeEvent();
            }
            Debug.LogWarning("gimmick input script not found in parent object!! Please add the gimmick input script to the parent object or place it as a child object of the object with gimmick input.");
        }
    }
    private void OnValidate()
    {

        GimmickInput GmmiInut = GetComponentInParent<GimmickInput>();
        if (GmmiInut != null)
        {
            GimmickInput = GmmiInut;
        }
        testbool = false;
    }
}
