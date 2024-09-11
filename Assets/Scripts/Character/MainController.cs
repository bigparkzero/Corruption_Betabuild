using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class MainController : MonoBehaviour
{
    //StateMachine fsm;

    public AnimationController anim;
    public LogicController logic;
    public CrowdControlController cc;
    //public PhysicsController physics;
    //public SkillController skill;
    //public Stats stats;

    //List<IUpdater> updaters = new List<IUpdater>();

    //public float walkSpeed = 4f;
    //public float sprintSpeed = 8f;

    //[HideInInspector] public bool isGameStopped;

    //protected virtual void Awake()
    //{
    //    CharacterController controller = GetComponent<CharacterController>();

    //    //fsm = new StateMachine(this);
    //    //
    //    //anim = new AnimationController(GetComponentInChildren<Animator>());
    //    //logic = new LogicController(transform.forward, this, controller);
    //    //cc = new CrowdControlController(this);
    //    //physics = new PhysicsController(transform, controller);
    //    //skill = new SkillController(this);
    //    //
    //    //updaters.Add(fsm);
    //    //updaters.Add(cc);
    //}

    //protected virtual void Update()
    //{
    //    if (isGameStopped) return;

    //    foreach (IUpdater updater in updaters)
    //    {
    //        updater.Update();
    //    }
    //}

    //public void SetInvincibility(bool isInvincible)
    //{
    //    stats.isInvincible = isInvincible;
    //}

    //public bool DoSkill(KeyBind.Action input)
    //{
    //    return skill.DoSkill(input);
    //}

    //public void StopSkill()
    //{
    //    skill.ForceStopSkill();
    //}

    //public void Move(int h, int v, bool isSprint)
    //{
    //    if (h == 0 && v == 0) return;
    //    float speed = walkSpeed;
    //    if (isSprint) speed = sprintSpeed;
    //    //Vector3 camBased = GetCamBasedDirection(h, v);
    //    //physics.MoveHorizontal(camBased, speed);
    //    //
    //    //Quaternion targetRotation = Quaternion.LookRotation(camBased);
    //    //SetTargetRotation(targetRotation);
    //}

    //public void MoveVertical()
    //{
    //    physics.MoveVertical();
    //}

    //public void ApplyGravity()
    //{
    //    physics.ApplyGravity();
    //}

    //public void SetForcedVerticalVelocity(float velocity)
    //{
    //    physics.SetVerticalVelocity(velocity);
    //}

    //public bool IsHeadColliding()
    //{
    //    return physics.IsHeadColliding();
    //}

    //public bool IsFallingDown()
    //{
    //    return physics.IsFallingDown();
    //}

    //public bool IsGrounded()
    //{
    //    bool isGrounded = physics.IsGrounded();

    //    return isGrounded;
    //}

    //public bool IsNearGround()
    //{
    //    return physics.IsNearGround();
    //}

    //public void SetTargetRotation(Quaternion targetRotation)
    //{
    //    physics.SetTargetRotation(targetRotation);
    //    logic.SetLogicalForward(targetRotation);
    //}

    //public void Turn()
    //{
    //    physics.Turn();
    //}

    //public void TurnImmediately(int h, int v)
    //{
    //    if (h == 0 && v == 0) return;
    //    //Vector3 camBased = GetCamBasedDirection(h, v);
    //    //Quaternion targetRotation = Quaternion.LookRotation(camBased);
    //    //SetTargetRotation(targetRotation);
    //    //physics.TurnImmediately();
    //}
}
