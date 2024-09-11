using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMainController : MainController
{
    public Transform targetPosition;
    public GameObject testSkillCollider;

    CharacterController controller;

    BehaviorTree ai;

    public PlayerMainController target;

    public Vector2 DISTANCE_LIMIT;

    public float DETECTION_RANGE;

    List<IUpdater> updaters = new List<IUpdater>();

    [HideInInspector] public bool isGameStopped;
    public EnemySpawner spawner;

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();

        anim = new AnimationController(GetComponentInParent<Animator>());
        cc = new CrowdControlController(this);
        logic = new LogicController(transform.forward, this, controller);

        ai = new BehaviorTree(this);

        updaters.Add(ai);
        updaters.Add(cc);
    }

    private void Update()
    {
        if (isGameStopped) return;

        logic.SetLogicalForward(transform.rotation);

        foreach (IUpdater updater in updaters)
        {
            updater.Update();
        }
    }

    public void CancelAttack()
    {
        anim.ResetTrigger("Attack");
    }

    public void Chase()
    {
        if (target == null) return;

        Vector3 offset = (transform.position - target.transform.position).normalized * (DISTANCE_LIMIT.x + DISTANCE_LIMIT.y)/2;
        targetPosition.position = target.transform.position + offset;
        anim.SetFloat("MoveZ", 1);
    }

    public void Backstep()
    {
        anim.SetTrigger("Backstep");
    }

    public void SetTarget(PlayerMainController target)
    {
        this.target = target;
    }

    public void DoIdle()
    {
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Backstep");
        anim.SetFloat("MoveZ", 0);
        targetPosition.position = transform.position;
    }

    public void Attack()
    {
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Backstep");
        anim.SetFloat("MoveZ", 0);

        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir.normalized);

        anim.SetTrigger("Attack");
        testSkillCollider.SetActive(true);
    }

    public void Die()
    {
        transform.root.gameObject.SetActive(false);
    }
}
