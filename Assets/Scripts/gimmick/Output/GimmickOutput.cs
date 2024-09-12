using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GimmickOutput : MonoBehaviour
{
    public bool isDone;
    public abstract void Act();
}
