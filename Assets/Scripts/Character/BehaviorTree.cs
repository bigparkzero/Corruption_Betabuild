using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region base
public enum NodeState
{
    Running,
    Success,
    Failure
}

public abstract class Node
{
    public MonsterMainController main;

    // 노드의 상태를 나타내는 변수
    protected NodeState _nodeState;

    // 이 노드의 자식 노드들
    protected List<Node> children = new List<Node>();

    // 자식 노드를 추가하는 함수
    public void AddChild(Node child)
    {
        child.main = BehaviorTree.currentMain;
        children.Add(child);
    }

    // 노드의 상태를 반환하는 함수
    public NodeState GetNodeState()
    {
        return _nodeState;
    }

    // 각 노드에서 구현해야 할 추상 메서드
    public abstract NodeState Evaluate();
}

public class Selector : Node
{
    public override NodeState Evaluate()
    {
        foreach (Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                    _nodeState = NodeState.Success;
                    return _nodeState;
                case NodeState.Running:
                    _nodeState = NodeState.Running;
                    return _nodeState;
                default:
                    continue;
            }
        }

        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class Sequence : Node
{
    public override NodeState Evaluate()
    {
        bool anyChildRunning = false;

        foreach (Node node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    _nodeState = NodeState.Failure;
                    return _nodeState;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    anyChildRunning = true;
                    continue;
                default:
                    _nodeState = NodeState.Success;
                    return _nodeState;
            }
        }

        _nodeState = anyChildRunning ? NodeState.Running : NodeState.Success;
        return _nodeState;
    }
}
#endregion

#region enemy
public class DeathSequence : Sequence
{
    public DeathSequence()
    {
        AddChild(new CheckHealth());
        AddChild(new DeathAction());
    }
}

public class CheckHealth : Node
{
    public override NodeState Evaluate()
    {
        if (main.GetComponent<EnemyStats>().hp <= 0)
        {
            _nodeState = NodeState.Success;
            return _nodeState;
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class DeathAction : Node
{
    public override NodeState Evaluate()
    {
        main.Die();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class CCSequence : Sequence
{
    public CCSequence()
    {
        AddChild(new CheckCC());
        AddChild(new CancelAttackAction());
    }
}

public class CheckCC : Node
{
    public override NodeState Evaluate()
    {
        if (main.cc.isCCed)
        {
            _nodeState = NodeState.Success;
            return _nodeState;
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class CancelAttackAction : Node
{
    public override NodeState Evaluate()
    {
        main.CancelAttack();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class AttackingSequence : Sequence
{
    public AttackingSequence()
    {
        AddChild(new AttackingCondition());
    }
}

public class AttackingCondition : Node
{
    public override NodeState Evaluate()
    {
        if (main.anim.GetNextState().IsName("Locomotion"))
        {
            main.spawner.attackerCount--;
        }
        if (main.anim.GetCurrentState().IsName("Attack"))
        {
            _nodeState = NodeState.Success;
            return _nodeState;
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class CombatSelector : Selector
{
    public CombatSelector()
    {
        AddChild(new DoAttackSequence());
        AddChild(new BackstepSequence());
        AddChild(new ChaseSequence());
        //AddChild(doattacksequence)
        AddChild(new WaitAction());
    }
}

public class DoAttackSequence : Sequence
{
    public DoAttackSequence()
    {
        AddChild(new CanAttackCondition());
        AddChild(new DoAttackAction());
    }
}

public class CanAttackCondition : Node
{
    public override NodeState Evaluate()
    {
        if (main.target != null)
        {
            if (main.spawner.attackerCount > 1)
            {
                _nodeState = NodeState.Failure;
                return _nodeState;
            }
            if (Vector3.Distance(main.transform.position, main.target.transform.position) <= 1.5f)
            {
                _nodeState = NodeState.Success;
                main.spawner.attackerCount++;
                return _nodeState;
            }
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class DoAttackAction : Node
{
    public override NodeState Evaluate()
    {
        main.Attack();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}


public class BackstepSequence : Sequence
{
    public BackstepSequence()
    {
        AddChild(new TooCloseCondition());
        AddChild(new BackstepAction());
    }
}

public class TooCloseCondition : Node
{
    public override NodeState Evaluate()
    {
        if (main.target != null)
        {
            if (Vector3.Distance(main.transform.position, main.target.transform.position) <= main.DISTANCE_LIMIT.x)
            {
                _nodeState = NodeState.Success;
                return _nodeState;
            }
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class BackstepAction : Node
{
    public override NodeState Evaluate()
    {
        main.Backstep();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class ChaseSequence : Sequence
{
    public ChaseSequence()
    {
        AddChild(new TooFarCondition());
        AddChild(new ChaseAction());
    }
}

public class TooFarCondition : Node
{
    public override NodeState Evaluate()
    {
        if (main.target != null)
        {
            if (Vector3.Distance(main.transform.position, main.target.transform.position) >= main.DISTANCE_LIMIT.y)
            {
                _nodeState = NodeState.Success;
                return _nodeState;
            }
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class ChaseAction : Node
{
    public override NodeState Evaluate()
    {
        main.Chase();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class WaitAction : Node
{
    public override NodeState Evaluate()
    {
        if (main.target == null)
        {
            _nodeState = NodeState.Failure;
            return _nodeState;
        }
        Vector3 dir = main.target.transform.position - main.transform.position;
        dir.y = 0;
        main.transform.rotation = Quaternion.LookRotation(dir.normalized);
        main.DoIdle();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class DetectionSequence : Sequence
{
    public DetectionSequence()
    {
        AddChild(new DetectCondition());
        AddChild(new IdleAction());
    }
}

public class DetectCondition : Node
{
    public override NodeState Evaluate()
    {
        Collider[] player = Physics.OverlapSphere(main.transform.position, main.DETECTION_RANGE, LayerMask.GetMask("Player"));

        if (player.Length > 0)
        {
            PlayerMainController target = player[0].transform.root.GetComponent<PlayerMainController>();
            main.SetTarget(target);
            _nodeState = NodeState.Success;
            return _nodeState;
        }
        _nodeState = NodeState.Failure;
        return _nodeState;
    }
}

public class IdleAction : Node
{
    public override NodeState Evaluate()
    {
        main.DoIdle();
        _nodeState = NodeState.Success;
        return _nodeState;
    }
}

public class RootNode : Selector
{
    public RootNode(MonsterMainController main)
    {
        this.main = main;

        AddChild(new DeathSequence());
        AddChild(new CCSequence());
        AddChild(new AttackingSequence());
        AddChild(new CombatSelector());
        AddChild(new DetectionSequence());
    }
}
#endregion

public class BehaviorTree : IUpdater
{
    public static MonsterMainController currentMain;

    private Node _rootNode;

    float thinkCooldown = 0;
    float UPDATE_TERM = 0.2f;

    public BehaviorTree(MonsterMainController main)
    {
        currentMain = main;

        // Behavior Tree의 루트 노드 초기화
        _rootNode = new RootNode(main);
    }

    public void Update()
    {
        if (thinkCooldown > 0)
        {
            thinkCooldown -= Time.deltaTime;
            return;
        }

        // 매 프레임마다 Behavior Tree 평가
        _rootNode.Evaluate();

        thinkCooldown = UPDATE_TERM;
    }
}
