using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrowdControlController : IUpdater
{
    public MainController main { get; private set; }

    List<CrowdControl> container = new List<CrowdControl>();
    List<int> removeIndices = new List<int>();

    public bool isCCed
    {
        get
        {
            return container.Count > 0;
        }
    }

    public CrowdControlController(MainController main)
    {
        this.main = main;
    }

    public void Update()
    {
        if (!isCCed)
        {
            main.anim.SetBool("CCed", false);
        }

        for(int i = 0; i < container.Count; i++)
        {
            var c = container[i];
            c.OnStay();
            c.duration -= Time.deltaTime;

            if (c.duration <= 0)
            {
                removeIndices.Add(i);
            }
        }

        for (int i = removeIndices.Count - 1; i >= 0; i--)
        {
            container[i].OnExit();
            container.RemoveAt(i);
        }

        removeIndices.Clear();
    }

    public void ApplyCC(CrowdControl cc)
    {
        cc.controller = this;
        container.Add(cc);
        cc.OnEnter();
    }
}

[Serializable]
public struct CrowdControlData
{
    public Vector3 dir;
    public CrowdControl.Type type;
    public float duration;
}

public abstract class CrowdControl
{
    public CrowdControlController controller;
    
    public enum Type
    {
        None,
        Stagger,
        Knockback,
    }

    public Type type
    {
        get;
        protected set;
    }

    public float duration;

    public virtual void OnEnter() { }
    public virtual void OnStay() { }
    public virtual void OnExit() { }
}

public class Stagger : CrowdControl
{
    Vector3 dir;

    public Stagger(float duration, Vector3 dir)
    {
        type = Type.Stagger;
        this.duration = duration;
        this.dir = dir;
    }

    public override void OnEnter()
    {
        string stateName;
        if (Vector3.Dot(controller.main.logic.logicalForward, dir) <= 0)
        {
            stateName = "Stagger from Back";
        }
        else
        {
            stateName = "Stagger from Forward";
        }
        controller.main.anim.CrossFade(stateName, 0);
        controller.main.anim.SetBool("CCed", true);
    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {

    }
}

public class Knockback : CrowdControl
{
    Vector3 dir;
    float distance;

    public Knockback(float duration, Vector3 dir, float distance)
    {
        type = Type.Knockback;
        this.duration = duration;
        this.dir = dir;
        this.distance = distance;
    }

    public override void OnEnter()
    {

    }

    public override void OnStay()
    {

    }

    public override void OnExit()
    {

    }
}