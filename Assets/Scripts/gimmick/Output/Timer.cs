using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Timer : GimmickOutput
{
    public float time;
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
