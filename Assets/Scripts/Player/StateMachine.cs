using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public enum Type
    {
        Locomotion,
        Jump,
        Air,
        Roll,
        Dash,
        Wall,
        Attack,
        CC,
        Interact,

        None
    }

    public Type type
    {
        get;
        protected set;
    }

    public static State CreateState(Type type)
    {
        State state = null;
        switch (type)
        {
            case Type.Locomotion:
                state = new LocomotionState();
                break;
            case Type.Jump:
                state = new JumpState();
                break;
            case Type.Air:
                state = new AirState();
                break;
            case Type.Roll:
                state = new RollState();
                break;
            case Type.Dash:
                state = new DashState();
                break;
            case Type.Wall:
                state = new WallState();
                break;
            case Type.Attack:
                state = new AttackState();
                break;
            case Type.CC:
                state = new CCState();
                break;
            case Type.Interact:
                state = new InteractState();
                break;
        }
        return state;
    }

    public abstract void OnEnter(PlayerMainController player);
    public abstract void OnStay(PlayerMainController player);
    public abstract void OnExit(PlayerMainController player);

    public abstract bool IsTransition(PlayerMainController player, out Type next);
}

public class LocomotionState : State
{
    bool isJumpActivated;

    public LocomotionState()
    {
        type = Type.Locomotion;
    }

    public override void OnEnter(PlayerMainController player)
    {
        isJumpActivated = false;

        player.anim.SetBool("IsGrounded", true);

        player.jumpGauge = player.DEFAULT_JUMP_GAUGE;
    }

    public override void OnExit(PlayerMainController player)
    {
        player.MAX_JUMP_HEIGHT = 8f;
        if (player.jumpGauge > 8) player.jumpGauge = 8;
    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();

        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        bool isSprint = Input.GetKey(KeyBind.Sprint);
        if (isSprint)
        {
            player.MAX_JUMP_HEIGHT = 40f;
        }
        else
        {
            player.MAX_JUMP_HEIGHT = 8f;
        }

        if (Input.GetKey(KeyBind.Jump)) player.jumpGauge = Mathf.Min(player.jumpGauge + 2 * player.MAX_JUMP_HEIGHT * Time.deltaTime, player.MAX_JUMP_HEIGHT);
        if (Input.GetKeyUp(KeyBind.Jump))
        {
            isJumpActivated = true;
        }
        player.Move(h, v, isSprint);
        player.MoveVertical();
        player.ApplyGravity();
        player.Turn();

        float moveZ = 0;
        if (v != 0 || h != 0) moveZ = isSprint ? 1 : 0.5f;

        player.anim.SetFloat("MoveZ", moveZ);
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        Collider wall = player.GetCollidedWall(h, v);

        if (Input.GetKey(KeyBind.Roll))
        {
            player.TurnImmediately(h, v);

            next = Type.Roll;
            return true;
        }
        else if (isJumpActivated || Input.GetKeyUp(KeyBind.Jump))
        {
            player.TurnImmediately(h, v);

            next = Type.Jump;
            return true;
        }
        else if (!player.IsGrounded())
        {
            player.anim.SetTrigger("Fall");
            next = Type.Air;
            return true;
        }
        else if (Input.GetKey(KeyBind.Sprint) && wall != null)
        {
            player.anim.SetTrigger("WallRide");
            player.attachedWall = wall;
            next = Type.Wall;
            return true;
        }
        else if (Input.GetKeyDown(KeyBind.BasicSkill))
        {
            if (player.DoSkill(KeyBind.Action.BasicSkill))
            {
                player.TurnImmediately(h, v);
                player.anim.SetTrigger("BasicSkill");

                next = Type.Attack;
                return true;
            }
        }
        //else if (Input.GetKeyDown(KeyBind.interact))
        //{
        //    next = Type.Interact;
        //    return true;
        //}
        next = Type.None;
        return false;
    }
}

public class JumpState : State
{
    public JumpState()
    {
        type = Type.Jump;
    }

    public override void OnEnter(PlayerMainController player)
    {
        player.Jump();

        player.anim.SetBool("IsGrounded", false);
        player.anim.SetTrigger("Jump");

        player.jumpGauge = player.DEFAULT_JUMP_GAUGE;
    }

    public override void OnExit(PlayerMainController player)
    {
        if (player.anim.GetNextState().IsName("Locomotion"))
        {
            player.anim.CrossFade("Land", 0);
        }
    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();

        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        bool isSprint = Input.GetKey(KeyBind.Sprint);

        player.HandleCamera();
        player.Move(h, v, isSprint);
        player.MoveVertical();
        if (!player.IsGrounded()) player.ApplyGravity();
        player.Turn();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        Collider wall = player.GetCollidedWall(h, v);

        if (Input.GetKeyDown(KeyBind.Dash))
        {
            next = Type.Dash;
            return true;
        }
        else if (player.IsHeadColliding())
        {
            next = Type.Air;
            return true;
        }
        else if (player.IsFallingDown())
        {
            player.anim.SetTrigger("Fall");
            next = Type.Air;
            return true;
        }
        else if (Input.GetKey(KeyBind.Sprint) && wall != null)
        {
            player.anim.SetTrigger("WallRide");
            player.attachedWall = wall;
            next = Type.Wall;
            return true;
        }

        next = Type.None;
        return false;
    }
}

public class AirState : State
{
    public AirState()
    {
        type = Type.Air;
    }

    public override void OnEnter(PlayerMainController player)
    {
        player.anim.SetBool("IsGrounded", false);
        player.jumpGauge = player.DEFAULT_JUMP_GAUGE;
    }

    public override void OnExit(PlayerMainController player)
    {

    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();

        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        bool isSprint = Input.GetKey(KeyBind.Sprint);

        player.HandleCamera();
        player.Move(h, v, isSprint);
        player.MoveVertical();
        player.ApplyGravity();
        player.Turn();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        Collider wall = player.GetCollidedWall(h, v);

        if (Input.GetKeyDown(KeyBind.Dash))
        {
            next = Type.Dash;
            return true;
        }
        else if (player.IsGrounded())
        {
            next = Type.Locomotion;
            return true;
        }
        else if (Input.GetKey(KeyBind.Sprint) && wall != null)
        {
            player.anim.SetTrigger("WallRide");
            player.attachedWall = wall;
            next = Type.Wall;
            return true;
        }
        next = Type.None;
        return false;
    }
}

public class RollState : State
{
    public RollState()
    {
        type = Type.Roll;
    }

    public override void OnEnter(PlayerMainController player)
    {
        player.anim.CrossFade("Roll", 0);
    }

    public override void OnExit(PlayerMainController player)
    {

    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();

        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        player.HandleCamera();
        
        player.ApplyGravity();
        player.Roll(h, v);
        player.MoveVertical();
        player.Turn();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        if (player.anim.GetNextState().IsName("Locomotion"))
        {
            next = Type.Locomotion;
            return true;
        }
        else if (player.anim.GetNextState().IsName("Fall"))
        {
            player.anim.SetTrigger("Fall");
            next = Type.Air;
            return true;
        }
        next = Type.None;
        return false;
    }
}

public class DashState : State
{
    public DashState()
    {
        type = Type.Dash;
    }

    public override void OnEnter(PlayerMainController player)
    {
        player.anim.SetTrigger("Dash");
        player.SetForcedVerticalVelocity(0);
    }

    public override void OnExit(PlayerMainController player)
    {

    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();

        player.Dash();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        if (player.anim.GetNextState().IsName("Fall"))
        {
            next = Type.Air;
            return true;
        }
        next = Type.None;
        return false;
    }
}

public class WallState : State
{

    public WallState()
    {
        type = Type.Wall;
    }

    public override void OnEnter(PlayerMainController player)
    {

    }

    public override void OnExit(PlayerMainController player)
    {

    }

    //TODO: 임시 처리중. 제대로 알고리즘 설계할 것.
    public override void OnStay(PlayerMainController player)
    {
        player.anim.SetFloat("MoveZ", 1);

        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        Collider wall = player.GetCollidedWall(h, v);

        player.attachedWall = wall;

        if (wall != null)
        {
            if (v > 0)
            {
                player.transform.position += Vector3.up * player.dashSpeed * Time.deltaTime;
            }
        }
        else
        {
            player.SetForcedVerticalVelocity(5f);
        }
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        if (player.attachedWall == null)
        {
            next = Type.Air;
            return true;
        }

        next = Type.None;
        return false;
    }
}

public class AttackState : State
{
    public AttackState()
    {
        type = Type.Attack;
    }

    public override void OnEnter(PlayerMainController player)
    {
        foreach (GameObject trail in player.trails)
        {
            trail.SetActive(true);
        }
    }

    public override void OnExit(PlayerMainController player)
    {
        foreach (GameObject trail in player.trails)
        {
            trail.SetActive(false);
        }
    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        int h = 0;
        int v = 0;
        if (Input.GetKey(KeyBind.Move_forward)) v += 1;
        if (Input.GetKey(KeyBind.Move_backward)) v -= 1;
        if (Input.GetKey(KeyBind.Move_left)) h -= 1;
        if (Input.GetKey(KeyBind.Move_right)) h += 1;

        //Collider wall = player.IsWallCollided(h, v);

        if (player.IsGrounded() && Input.GetKey(KeyBind.Roll))
        {
            player.StopSkill();
            player.TurnImmediately(h, v);

            next = Type.Roll;
            return true;
        }
        if (Input.GetKeyDown(KeyBind.BasicSkill))
        {
            if (player.DoSkill(KeyBind.Action.BasicSkill))
            {
                player.TurnImmediately(h, v);
                player.anim.SetTrigger("BasicSkill");
                player.physics.SetVerticalVelocity(0);

                next = Type.Attack;
                return true;
            }

            next = Type.None;
            return false;
        }
        else if (player.anim.GetNextState().IsName("Locomotion"))
        {
            player.anim.ResetTrigger("BasicSkill");
            player.anim.ResetTrigger("SpecialSkill");
            next = Type.Locomotion;
            return true;
        }

        next = Type.None;
        return false;
    }
}

public class CCState : State
{
    public CCState()
    {
        type = Type.CC;
    }

    public override void OnEnter(PlayerMainController player)
    {

    }

    public override void OnExit(PlayerMainController player)
    {

    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        next = Type.None;
        return false;
    }
}

public class InteractState : State
{
    public InteractState()
    {
        type = Type.Interact;
    }

    public override void OnEnter(PlayerMainController player)
    {

    }

    public override void OnExit(PlayerMainController player)
    {

    }

    public override void OnStay(PlayerMainController player)
    {
        player.HandleCamera();
    }

    public override bool IsTransition(PlayerMainController player, out Type next)
    {
        next = Type.None;
        return false;
    }
}

[System.Serializable]
public class StateMachine : IUpdater
{
    PlayerMainController player;
    private static readonly List<State> states1 = new List<State>();
    List<State> states = states1;

    public State current { get; private set; }

    public StateMachine(PlayerMainController player)
    {
        this.player = player;
        Init();
    }

    void Init()
    {
        for (int i = 0; i < (int)State.Type.None; i++)
        {
            State.Type type = (State.Type)i;
            states.Add(State.CreateState(type));
        }

        int loco = (int)State.Type.Locomotion;
        current = states[loco];
    }

    public void Transition(State.Type type)
    {
        current?.OnExit(player);
        current = states[(int)type];
        current?.OnEnter(player);
    }

    public void Update()
    {
        if (current != null && current.IsTransition(player, out State.Type type))
        {
            Transition(type);
        }

        current?.OnStay(player);
    }
}
