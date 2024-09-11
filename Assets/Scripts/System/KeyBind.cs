using System.Collections.Generic;
using UnityEngine;

public static class KeyBind
{
    public enum Action
    {
        Move_Forward,
        Move_Backward,
        Move_Left,
        Move_Right,
        Sprint,
        Jump,
        Roll,
        Dash,
        Interact,
        BasicSkill,
        Special_Skill,
        Morph,
    }

    public static Dictionary<Action, KeyCode> keys = new Dictionary<Action, KeyCode>();

    public static KeyCode Move_forward => keys[Action.Move_Forward];
    public static KeyCode Move_backward => keys[Action.Move_Backward];
    public static KeyCode Move_left => keys[Action.Move_Left];
    public static KeyCode Move_right => keys[Action.Move_Right];

    public static KeyCode Sprint => keys[Action.Sprint];
    public static KeyCode Jump => keys[Action.Jump];
    public static KeyCode Roll => keys[Action.Roll];
    public static KeyCode Dash => keys[Action.Dash];

    public static KeyCode Interact => keys[Action.Interact];

    public static KeyCode BasicSkill => keys[Action.BasicSkill];
    public static KeyCode Special_skill => keys[Action.Special_Skill];

    public static KeyCode Morph => keys[Action.Morph];

    static KeyBind()
    {
        keys[Action.Move_Forward] = KeyCode.W;
        keys[Action.Move_Backward] = KeyCode.S;
        keys[Action.Move_Left] = KeyCode.A;
        keys[Action.Move_Right] = KeyCode.D;


        keys[Action.Sprint] = KeyCode.LeftShift;
        keys[Action.Jump] = KeyCode.V;
        keys[Action.Roll] = KeyCode.Space;
        keys[Action.Dash] = KeyCode.LeftControl;

        keys[Action.Interact] = KeyCode.G;

        keys[Action.BasicSkill] = KeyCode.Mouse0;
        keys[Action.Special_Skill] = KeyCode.Alpha1;

        keys[Action.Morph] = KeyCode.R;
    }
}