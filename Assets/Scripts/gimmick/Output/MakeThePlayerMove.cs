using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MakeThePlayerMove : GimmickOutput
{
    public PlayerMainController player;
    public Animator playerAn;
    public CharacterController characterController; // CharacterController 참조 추가
    public string assetPath = "Assets/Animations/Player/moveAnimator.controller";
    public List<Transform> waypoint;
    AnimatorController movecontroller;
    AnimatorController playeranimatorcontroller;

    private void Start()
    {
        SetUp();
    }
    void SetUp()
    {
        //player = player.gameObject.GetComponent<PlayerMainController>();
       // playerAn = GetComponent<Animator>();
        //characterController = player.gameObject.GetComponent<CharacterController>(); // CharacterController 컴포넌트 참조
        RuntimeAnimatorController runtimeAnimatorController = playerAn.runtimeAnimatorController;
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
        LoadMyAsset();
    }
    private void OnValidate()
    {
        LoadMyAsset();
    }

    public void LoadMyAsset()
    {
        // assetPath 경로에 있는 에셋을 로드합니다
        movecontroller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);

        if (movecontroller != null)
        {
            Debug.Log("에셋 로드 성공: " + movecontroller.name);
        }
        else
        {
            Debug.LogError("에셋 로드 실패: 경로를 확인하세요.");
        }
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
        playerAn.runtimeAnimatorController = movecontroller;

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
                    if (distance > 0.5f) // 목표 waypoint에 도달하지 않은 경우 이동
                    {
                        Vector3 moveDirection = direction.normalized;
                        characterController.Move(moveDirection * Time.deltaTime * 5f); // 이동 속도 조절
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
        playerAn.SetBool("isarrival", true);
        StartCoroutine(CheckAnimationPlayback());
    }
    IEnumerator CheckAnimationPlayback()
    {
        bool isAnimationPlaying = true;

        while (isAnimationPlaying)
        {
            // 애니메이션의 현재 상태를 가져오기
            AnimatorStateInfo stateInfo = playerAn.GetCurrentAnimatorStateInfo(0);

            // 애니메이션이 현재 재생 중인지 확인
            if (stateInfo.IsName("Blend Tree") && stateInfo.normalizedTime >= 1.0f)
            {
                // 애니메이션 재생이 완료되었으면
                isAnimationPlaying = false;
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        actionExit();
    }
    public void actionExit()
    {
        player.enabled = true;
        playerAn.runtimeAnimatorController = playeranimatorcontroller;
        isDone = true;
    }
}
