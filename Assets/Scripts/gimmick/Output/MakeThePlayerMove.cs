using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class MakeThePlayerMove : GimmickOutput
{
    public PlayerMainController player;
    public List<Animator> playerAn; // ���� �ִϸ����͵��� ����Ʈ�� ����
    public CharacterController characterController; // CharacterController ���� �߰�
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

        // ��� �ִϸ����Ϳ� ������ �ִϸ����� ��Ʈ�ѷ� ����
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
                    if (distance > 0.1f) // ��ǥ waypoint�� �������� ���� ��� �̵�
                    {
                        Vector3 moveDirection = direction.normalized;
                        characterController.Move(moveDirection * Time.deltaTime * 3.5f); // �̵� �ӵ� ����
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
        // ��� �ִϸ����Ϳ� ���� �Ϸ� �ִϸ��̼��� ����
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
            // ��� �ִϸ����Ͱ� �Ϸ�Ǿ����� Ȯ��
            
            foreach (var animator in playerAn)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Blend Tree") && stateInfo.normalizedTime >= 0.975f)
                {
                    isAnimationPlaying = false;
                    break;
                }
            }

            // ���� �����ӱ��� ���
            yield return null;
        }

        actionExit();
    }

    public void actionExit()
    {
        player.enabled = true;

        // ��� �ִϸ����Ϳ� ������ �ִϸ����� ��Ʈ�ѷ��� �ٽ� ����
        foreach (var animator in playerAn)
        {
            animator.runtimeAnimatorController = playeranimatorcontroller;
        }

        isDone = true;
    }
}
