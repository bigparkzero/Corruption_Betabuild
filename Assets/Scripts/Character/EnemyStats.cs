using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    //NpcController controller;

    protected override void Start()
    {
        base.Start();
        //controller = GetComponent<NpcController>();
    }

    public override void Die()
    {
        base.Die();
        //controller.death = true;
    }
}
