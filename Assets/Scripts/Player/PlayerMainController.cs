using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainController : MainController
{
    #region to use dictionary in inspector
    [Serializable]
    public class MorphBodyMapping
    {
        public Morph morph;
        public GameObject body;
    }
    [SerializeField] List<MorphBodyMapping> _bodies = new List<MorphBodyMapping>();
    #endregion

    [Flags]
    public enum Morph
    {
        NONE            = 0,
        LEFT_HAND       = 1 << 0,
        RIGHT_HAND      = 1 << 1,
        BACK            = 1 << 2,
        LEGS            = 1 << 3,
    }

    public Transform vCamera;
    public List<GameObject> trails
    {
        get
        {
            return bodies[currentMorph].GetComponent<PlayerMorphData>().trails;
        }
    }

    Dictionary<Morph, GameObject> bodies = new Dictionary<Morph, GameObject>();

    StateMachine fsm;
    //public CameraController cam;

    public PhysicsController physics;
    public SkillController skill;

    PlayerStats stats;

    public Transform cameraLookAt;
    public Transform cameraRig;

    public Transform skillPosition; //TODO: 무기마다, 또 적절한 위치 전부 List나 뭐 어떻게 저장할수 있어야할듯.

    public Morph currentMorph = Morph.NONE;

    List<IUpdater> updaters = new List<IUpdater>();

    public float jumpGauge;
    public float DEFAULT_JUMP_GAUGE = 2.5f;
    public float MAX_JUMP_HEIGHT = 8f;

    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float rollSpeed = 2f;
    public float dashSpeed = 10f;

    public Collider attachedWall;

    [HideInInspector] public bool isGameStopped;

    private void Awake()
    {
        CharacterController controller = GetComponent<CharacterController>();

        fsm = new StateMachine(this);
        //cam = new CameraController(vCamera, cameraRig, cameraLookAt);
        vCamera = FindFirstObjectByType<CinemachineFreeLook>().transform;
        if (vCamera == null) throw new System.Exception("no freelook camera is found in this scene.");

        anim = new AnimationController(GetComponentInChildren<Animator>());
        logic = new LogicController(transform.forward, this, controller);
        cc = new CrowdControlController(this);
        physics = new PhysicsController(transform, controller);
        skill = new SkillController(this);
        stats = GetComponent<PlayerStats>();

        updaters.Add(fsm);
        updaters.Add(cc);
        updaters.Add(stats);

        foreach (var mapping in _bodies)
        {
            if (!bodies.ContainsKey(mapping.morph))
            {
                Morph morph = mapping.morph;
                if ((int)morph == -1) morph = Morph.LEFT_HAND | Morph.RIGHT_HAND | Morph.BACK | Morph.LEGS;
                bodies.Add(morph, mapping.body);
            }
            else
            {
                throw new Exception("player morph body setting failed! check inspector.");
            }
        }
    }

    private void Start()
    {
        jumpGauge = DEFAULT_JUMP_GAUGE;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(vCamera.gameObject);
        DontDestroyOnLoad(Camera.main.gameObject);
        DontDestroyOnLoad(GameObject.Find("Directional Light").gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyBind.Morph))
        {
            GameManager.Instance.SetActiveMorphScreen(true);
        }
        else if (Input.GetKeyUp(KeyBind.Morph))
        {
            GameManager.Instance.SetActiveMorphScreen(false);
        }

        if (isGameStopped) return;

        foreach (IUpdater updater in updaters)
        {
            updater.Update();
        }
    }

    public void SetInvincibility(bool isInvincible)
    {
        stats.isInvincible = isInvincible;
    }

    public void DoMorph(Morph morph)
    {
        if (stats.GetMorph(morph))
        {
            bodies[currentMorph].SetActive(false);
            bodies[morph].SetActive(true);

            currentMorph = morph;

            Animator newAnim = bodies[currentMorph].GetComponent<Animator>();
            anim.ChangeAnimator(newAnim);
            anim.SetInteger("Morph", (int)morph);
        }
        else
        {
#if UNITY_EDITOR
            print($"not enough morph point. morph to '{morph.ToString()}' has failed.");
#endif
        }
    }

    public bool DoSkill(KeyBind.Action input)
    {
        return skill.DoSkill(input);
    }

    public void StopSkill()
    {
        skill.ForceStopSkill();
    }

    public void HandleCamera()
    {
        return;
        //cam.HandleMouseInput(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        //cam.LimitCameraDistance();
    }

    public void Move(int h, int v, bool isSprint)
    {
        if (h == 0 && v == 0) return;
        float speed = walkSpeed;
        if (isSprint) speed = sprintSpeed;
        Vector3 camBased = GetCamBasedDirection(h, v);
        physics.MoveHorizontal(camBased, speed);

        Quaternion targetRotation = Quaternion.LookRotation(camBased);
        SetTargetRotation(targetRotation);
    }

    public void Roll(int h, int v)
    {
        Vector3 dir;
        if (h == 0 && v == 0)
        {
            dir = logic.logicalForward;
        }
        else
        {
            dir = GetCamBasedDirection(h, v);
        }
        physics.MoveHorizontal(dir, rollSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        SetTargetRotation(targetRotation);
    }
    public void Dash()
    {
        Vector3 dir = logic.logicalForward;

        physics.MoveHorizontal(dir, dashSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        SetTargetRotation(targetRotation);
    }

    public void MoveVertical()
    {
        physics.MoveVertical();
    }

    Vector3 GetCameraForward()
    {
        Vector3 result = cameraLookAt.position - vCamera.position;
        result.y = 0;
        return result.normalized;
    }

    public Vector3 GetCamBasedDirection(int h, int v)
    {
        if (h == 0 && v == 0) return Vector3.zero;

        Vector3 camForward = GetCameraForward();
        Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

        return (camForward * v + camRight * h).normalized;
    }

    public void ApplyGravity()
    {
        physics.ApplyGravity();
    }

    public void Jump()
    {
        physics.SetVerticalVelocity(jumpGauge);
    }

    public void SetForcedVerticalVelocity(float velocity)
    {
        physics.SetVerticalVelocity(velocity);
    }

    public bool IsHeadColliding()
    {
        return physics.IsHeadColliding();
    }

    public bool IsFallingDown()
    {
        return physics.IsFallingDown();
    }

    public bool IsGrounded()
    {
        bool isGrounded = physics.IsGrounded();

        return isGrounded;
    }

    public Collider GetCollidedWall(int h, int v)
    {
        if (h == 0 && v == 0) return null;

        Collider result = null;
        Vector3 camBased = GetCamBasedDirection(h, v);
        result = physics.IsWallCollided(camBased);

        return result;
    }

    public bool IsNearGround()
    {
        return physics.IsNearGround();
    }

    public void SetTargetRotation(Quaternion targetRotation)
    {
        physics.SetTargetRotation(targetRotation);
        logic.SetLogicalForward(targetRotation);
    }

    public void Turn()
    {
        physics.Turn();
    }

    public void TurnImmediately(int h, int v)
    {
        if (h == 0 && v == 0) return;
        Vector3 camBased = GetCamBasedDirection(h, v);
        Quaternion targetRotation = Quaternion.LookRotation(camBased);
        SetTargetRotation(targetRotation);
        physics.TurnImmediately();
    }

    public void Interact()
    {

    }
}
