using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
#if UNITY_EDITOR
    public static readonly string VERSION = "build_2.0.0.3";
#endif

    #region Game State Machine

    abstract class GameState
    {
        public enum Type
        {
            Play,
            Pause,
            Morph,

            None
        }

        public Type type
        {
            get;
            protected set;
        }

        public static GameState CreateState(Type type)
        {
            GameState state = null;

            switch (type)
            {
                case Type.Play:
                    state = new PlayState();
                    break;
                case Type.Pause:
                    state = new PauseState();
                    break;
                case Type.Morph:
                    state = new MorphState();
                    break;
            }

            return state;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnStay();
    }

    class PlayState : GameState
    {
        public PlayState()
        {
            type = Type.Play;
        }

        public override void OnEnter()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Instance.playerMain.isGameStopped = false;
        }

        public override void OnExit()
        {

        }

        public override void OnStay()
        {

        }
    }

    class PauseState : GameState
    {
        public PauseState()
        {
            type = Type.Pause;
        }

        public override void OnEnter()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Instance.playerMain.isGameStopped = true;
        }

        public override void OnExit()
        {

        }

        public override void OnStay()
        {

        }
    }

    class MorphState : GameState
    {
        public MorphState()
        {
            type = Type.Morph;
        }

        public override void OnEnter()
        {
            Time.timeScale = 0;
            Instance.UI_MorphScreen.gameObject.SetActive(true);
            Instance.playerMain.isGameStopped = true;
        }

        public override void OnExit()
        {
            Time.timeScale = 1;
            var targetMorph = Instance.UI_MorphScreen.GetTargetMorph();
            Instance.playerMain.DoMorph(targetMorph);
            Instance.UI_MorphScreen.gameObject.SetActive(false);
        }

        public override void OnStay()
        {

        }
    }

    class GameStateMachine
    {
        private static readonly List<GameState> _states = new List<GameState>();
        List<GameState> states = _states;

        public GameState current { get; private set; }

        public GameStateMachine()
        {
            Init();
        }

        public void Transition(GameState.Type next)
        {
            current?.OnExit();
            current = states[(int)next];
            current?.OnEnter();
        }

        public void Update()
        {
            current.OnStay();
        }

        void Init()
        {
            for (int i = 0; i < (int)GameState.Type.None; i++)
            {
                GameState.Type type = (GameState.Type)i;
                states.Add(GameState.CreateState(type));
            }

            Transition(GameState.Type.Play);
        }
    }
    #endregion

    public PlayerMainController playerMain;
    public UI_MorphScreen UI_MorphScreen;

    GameStateMachine stateMachine;

    private void Start()
    {
        UI_MorphScreen.gameObject.SetActive(false);

        stateMachine = new GameStateMachine();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void SetActiveMorphScreen(bool active)
    {
        switch (stateMachine.current.type)
        {
            case GameState.Type.Play:
                if (active)
                {
                    stateMachine.Transition(GameState.Type.Morph);
                }
                break;
            case GameState.Type.Morph:
                if (!active)
                {
                    stateMachine.Transition(GameState.Type.Play);
                }
                break;
        }
    }
}
