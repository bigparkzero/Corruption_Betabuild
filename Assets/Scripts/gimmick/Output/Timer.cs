using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;



public class Timer : GimmickOutput
{
    private void Start()
    {
        isDone = false;
    }
    public void Act(float time)
    {
        StartCoroutine(Delay(time));
    }
    public IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        isDone = true;
    }
}
