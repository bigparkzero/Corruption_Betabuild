using UnityEngine;
using UnityEngine.UI;
using Morph = PlayerMainController.Morph;

public class UI_MorphScreen : MonoBehaviour
{
    public enum Slot
    {
        Center,
        Up,
        Down,
        Left,
        Right,
    }

    public RectTransform selectbox;
    public Image img_up;
    public Image img_down;
    public Image img_left;
    public Image img_right;

    float elapsedX;
    float elapsedY;

    const float BORDER = 0.15f;
    const float SHORT_BORDER = 0.1f;
    const float CANCEL_BORDER = 0.3f;

    float cooldown;

    public Slot current
    {
        get;
        private set;
    }

    public Morph targetMorphs;

    Vector2 UP_POS = new Vector2(0, 100f);
    Vector2 DOWN_POS = new Vector2(0, -100f);
    Vector2 LEFT_POS = new Vector2(-100f, 0);
    Vector2 RIGHT_POS = new Vector2(100f, 0);
    Vector2 CENTER_POS = Vector2.zero;

    private void OnEnable()
    {
        targetMorphs = Morph.NONE;

        img_up.color = Color.white;
        img_down.color = Color.white;
        img_left.color = Color.white;
        img_right.color = Color.white;

        ChangeSlot(Slot.Center);
        cooldown = 0;
    }

    void Update()
    {
        GetMouseInput();

        if (cooldown > 0)
        {
            cooldown -= Time.unscaledDeltaTime;
            return;
        }

        GetMouseMovement();

        CheckMousePosition();
    }

    void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch (current)
            {
                case Slot.Center:
                    GameManager.Instance.SetActiveMorphScreen(false);
                    break;
                case Slot.Up:
                    if ((targetMorphs & Morph.BACK) != 0)
                    {
                        targetMorphs &= ~Morph.BACK;
                        img_up.color = Color.cyan;
                    }
                    else
                    {
                        targetMorphs |= Morph.BACK;
                        img_up.color = Color.green;
                    }
                    break;
                case Slot.Down:
                    if ((targetMorphs & Morph.LEGS) != 0)
                    {
                        targetMorphs &= ~Morph.LEGS;
                        img_down.color = Color.cyan;
                    }
                    else
                    {
                        targetMorphs |= Morph.LEGS;
                        img_down.color = Color.green;
                    }
                    break;
                case Slot.Left:
                    if ((targetMorphs & Morph.LEFT_HAND) != 0)
                    {
                        targetMorphs &= ~Morph.LEFT_HAND;
                        img_left.color = Color.cyan;
                    }
                    else
                    {
                        targetMorphs |= Morph.LEFT_HAND;
                        img_left.color = Color.green;
                    }
                    break;
                case Slot.Right:
                    if ((targetMorphs & Morph.RIGHT_HAND) != 0)
                    {
                        targetMorphs &= ~Morph.RIGHT_HAND;
                        img_right.color = Color.cyan;
                    }
                    else
                    {
                        targetMorphs |= Morph.RIGHT_HAND;
                        img_right.color = Color.green;
                    }
                    break;
            }
        }
    }

    void GetMouseMovement()
    {
        elapsedX += Input.GetAxisRaw("Mouse X") * Time.unscaledDeltaTime;
        elapsedY += Input.GetAxisRaw("Mouse Y") * Time.unscaledDeltaTime;

        switch (current)
        {
            case Slot.Up:
                elapsedY = Mathf.Min(elapsedY, 0);
                break;
            case Slot.Down:
                elapsedY = Mathf.Max(elapsedY, 0);
                break;
            case Slot.Left:
                elapsedX = Mathf.Max(elapsedX, 0);
                break;
            case Slot.Right:
                elapsedX = Mathf.Min(elapsedX, 0);
                break;
        }
    }

    void CheckMousePosition()
    {
        switch (current)
        {
            case Slot.Center:
                if (elapsedY >= BORDER) ChangeSlot(Slot.Up);
                else if (elapsedY <= -BORDER) ChangeSlot(Slot.Down);
                else if (elapsedX >= BORDER) ChangeSlot(Slot.Right);
                else if (elapsedX <= -BORDER) ChangeSlot(Slot.Left);
                break;
            case Slot.Up:
                if (elapsedY <= -CANCEL_BORDER) ChangeSlot(Slot.Center);
                else if (elapsedX >= SHORT_BORDER) ChangeSlot(Slot.Right);
                else if (elapsedX <= -SHORT_BORDER) ChangeSlot(Slot.Left);
                break;
            case Slot.Down:
                if (elapsedY >= CANCEL_BORDER) ChangeSlot(Slot.Center);
                else if (elapsedX >= SHORT_BORDER) ChangeSlot(Slot.Right);
                else if (elapsedX <= -SHORT_BORDER) ChangeSlot(Slot.Left);
                break;
            case Slot.Left:
                if (elapsedY >= SHORT_BORDER) ChangeSlot(Slot.Up);
                else if (elapsedY <= -SHORT_BORDER) ChangeSlot(Slot.Down);
                else if (elapsedX >= CANCEL_BORDER) ChangeSlot(Slot.Center);
                break;
            case Slot.Right:
                if (elapsedY >= SHORT_BORDER) ChangeSlot(Slot.Up);
                else if (elapsedY <= -SHORT_BORDER) ChangeSlot(Slot.Down);
                else if (elapsedX <= -CANCEL_BORDER) ChangeSlot(Slot.Center);
                break;
        }
    }

    void ChangeSlot(Slot slot)
    {
        elapsedX = 0;
        elapsedY = 0;

        switch (current)
        {
            case Slot.Center:
                break;
            case Slot.Up:
                if ((targetMorphs & Morph.BACK) != 0) img_up.color = Color.green;
                else img_up.color = Color.white;
                break;
            case Slot.Down:
                if ((targetMorphs & Morph.LEGS) != 0) img_down.color = Color.green;
                else img_down.color = Color.white;
                break;
            case Slot.Left:
                if ((targetMorphs & Morph.LEFT_HAND) != 0) img_left.color = Color.green;
                else img_left.color = Color.white;
                break;
            case Slot.Right:
                if ((targetMorphs & Morph.RIGHT_HAND) != 0) img_right.color = Color.green;
                else img_right.color = Color.white;
                break;
        }

        current = slot;
        cooldown = 0.25f;

        switch (current)
        {
            case Slot.Center:
                selectbox.anchoredPosition = CENTER_POS;
                break;
            case Slot.Up:
                selectbox.anchoredPosition = UP_POS;
                if ((targetMorphs & Morph.BACK) != 0) img_up.color = Color.green;
                else img_up.color = Color.cyan;
                break;
            case Slot.Down:
                selectbox.anchoredPosition = DOWN_POS;
                if ((targetMorphs & Morph.LEGS) != 0) img_down.color = Color.green;
                else img_down.color = Color.cyan;
                break;
            case Slot.Left:
                selectbox.anchoredPosition = LEFT_POS;
                if ((targetMorphs & Morph.LEFT_HAND) != 0) img_left.color = Color.green;
                else img_left.color = Color.cyan;
                break;
            case Slot.Right:
                selectbox.anchoredPosition = RIGHT_POS;
                if ((targetMorphs & Morph.RIGHT_HAND) != 0) img_right.color = Color.green;
                else img_right.color = Color.cyan;
                break;
        }
    }

    public Morph GetTargetMorph()
    {
        return targetMorphs;
    }
}
