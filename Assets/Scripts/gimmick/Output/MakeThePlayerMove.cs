using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MakeThePlayerMove : GimmickOutput
{
    public PlayerMainController player;
    public Animator playerAn;
    public CharacterController characterController; // CharacterController ���� �߰�
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
        //characterController = player.gameObject.GetComponent<CharacterController>(); // CharacterController ������Ʈ ����
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
        // assetPath ��ο� �ִ� ������ �ε��մϴ�
        movecontroller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);

        if (movecontroller != null)
        {
            Debug.Log("���� �ε� ����: " + movecontroller.name);
        }
        else
        {
            Debug.LogError("���� �ε� ����: ��θ� Ȯ���ϼ���.");
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
                    // ���� �÷��̾��� ��ġ ���̸� ��� (Y���� ����)
                    Vector3 direction = waypoint[i].position - player.transform.position;
                    direction.y = 0; // Y�� ���̴� ���� (���� ���⸸ �ٶ󺸰� ��)

                    // ��� �������� ȸ��
                    if (direction != Vector3.zero) // ȸ���� ������ �����ϴ��� Ȯ��
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, Time.deltaTime * 5f); // �ε巴�� ȸ��
                    }

                    // �̵�
                    float distance = Vector3.Distance(player.transform.position, waypoint[i].position);
                    if (distance > 0.5f) // ��ǥ waypoint�� �������� ���� ��� �̵�
                    {
                        Vector3 moveDirection = direction.normalized;
                        characterController.Move(moveDirection * Time.deltaTime * 5f); // �̵� �ӵ� ����
                    }
                    else
                    {
                        break; // while ���� �����ϰ� ���� waypoint�� �̵�
                    }
                }
                yield return null;
            }
        }

        // ��� waypoint�� ��ȸ�� �� ������ ����
        OnWaypointsComplete();
    }

    // ��� waypoint�� ��ȸ�� �� ������ ����
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
            // �ִϸ��̼��� ���� ���¸� ��������
            AnimatorStateInfo stateInfo = playerAn.GetCurrentAnimatorStateInfo(0);

            // �ִϸ��̼��� ���� ��� ������ Ȯ��
            if (stateInfo.IsName("Blend Tree") && stateInfo.normalizedTime >= 1.0f)
            {
                // �ִϸ��̼� ����� �Ϸ�Ǿ�����
                isAnimationPlaying = false;
            }

            // ���� �����ӱ��� ���
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
