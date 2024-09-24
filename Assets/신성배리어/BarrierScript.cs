using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScript : MonoBehaviour
{
    MeshRenderer meshRenderer;
    private void Start()
    {
        a = 5;
        meshRenderer = GetComponent<MeshRenderer>();
    }
    int a;
    public void Count()
    {
        if (a != 0)
        {
            a--;
            meshRenderer.material.SetFloat("_Float", a);
        }
        else
        {
            Invoke("destroy",0.5f);
        }
    }
    private void destroy()
    {
        Destroy(gameObject);
    }
}
