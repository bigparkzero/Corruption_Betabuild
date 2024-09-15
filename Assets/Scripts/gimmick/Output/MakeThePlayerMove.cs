using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MakeThePlayerMove : GimmickOutput
{
    public PlayerMainController player;
    public List<Animator> playerAn; // 여러 애니메이터들을 리스트로 참조
    public CharacterController characterController; // CharacterController 참조 추가
    public List<Transform> waypoint;
    public AnimatorController movecontroller;
    AnimatorController playeranimatorcontroller;
    public float AnimationNum;

    private void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        if (playerAn != null && playerAn.Count > 0)
        {
            foreach (var animator in playerAn)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                if (runtimeAnimatorController != null)
                {
                    playeranimatorcontroller = runtimeAnimatorController as AnimatorController;
                    if (playeranimatorcontroller == null)
                    {
                        Debug.LogWarning("The runtimeAnimatorController is not of type AnimatorController.");
                    }
                }
                else
                {
                    Debug.LogWarning("The Animator does not have a runtimeAnimatorController assigned.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Animator list is empty or null.");
        }
    }

    public void SetAnimationNum(float num)
    {
        AnimationNum = num;
    }

    public void Act()
    {
        if (player.enabled)
        {
            StartCoroutine(moveplayer());
        }
    }

    IEnumerator moveplayer()
    {
        isDone = false;
        player.enabled = false;

        // 모든 애니메이터에 동일한 애니메이터 컨트롤러 적용
        foreach (var animator in playerAn)
        {
            animator.runtimeAnimatorController = movecontroller;
        }

        for (int i = 0; i < waypoint.Count; i++)
        {
            while (true)
            {
                if (waypoint[i] != null)
                {
                    // 대상과 플레이어의 위치 차이를 계산 (Y값을 무시)
                    Vector3 direction = waypoint[i].position - player.transform.position;
                    direction.y = 0; // Y축 차이는 무시 (수평 방향만 바라보게 함)

                    // 대상 방향으로 회전
                    if (direction != Vector3.zero) // 회전할 방향이 존재하는지 확인
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * 5f); // 부드럽게 회전
                    }

                    // 이동
                    float distance = Vector3.Distance(player.transform.position, waypoint[i].position);
                    if (distance > 0.1f) // 목표 waypoint에 도달하지 않은 경우 이동
                    {
                        Vector3 moveDirection = direction.normalized;
                        characterController.Move(moveDirection * Time.deltaTime * 3.5f); // 이동 속도 조절
                    }
                    else
                    {
                        break; // while 문을 종료하고 다음 waypoint로 이동
                    }
                }
                yield return null;
            }
        }

        // 모든 waypoint를 순회한 후 실행할 로직
        OnWaypointsComplete();
    }

    // 모든 waypoint를 순회한 후 실행할 로직
    void OnWaypointsComplete()
    {
        // 모든 애니메이터에 대해 완료 애니메이션을 설정
        foreach (var animator in playerAn)
        {
            animator.SetBool("isarrival", true);
            animator.SetFloat("BlendID", AnimationNum);
        }

        StartCoroutine(CheckAnimationPlayback());
    }

    IEnumerator CheckAnimationPlayback()
    {
        bool isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            // 모든 애니메이터가 완료되었는지 확인
            
            foreach (var animator in playerAn)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Blend Tree") && stateInfo.normalizedTime >= 0.975f)
                {
                    isAnimationPlaying = false;
                    break;
                }
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        actionExit();
    }

    public void actionExit()
    {
        player.enabled = true;

        // 모든 애니메이터에 원래의 애니메이터 컨트롤러를 다시 적용
        foreach (var animator in playerAn)
        {
            animator.runtimeAnimatorController = playeranimatorcontroller;
        }

        isDone = true;
    }
}
