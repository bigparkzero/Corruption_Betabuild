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
    public override void Act()
    {
        StartCoroutine(Delay());
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(time);
        isDone = true;
    }
}
