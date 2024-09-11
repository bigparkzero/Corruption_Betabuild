using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundPoint : MonoBehaviour
{
    public enum Type
    {
        Ground,
        Building,
    }

    public Type type;
}
