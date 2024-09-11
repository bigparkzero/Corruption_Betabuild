using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats : MonoBehaviour
{
    public float hp;
    public float maxHp;

    public bool isInvincible;

    protected virtual void Start()
    {
        hp = maxHp;
    }

    public virtual void Damaged(float damage, CrowdControlData ccData)
    {
        if (isInvincible)
        {
            return;
        }

        hp = Mathf.Max(hp - damage, 0);

        switch (ccData.type)
        {
            case CrowdControl.Type.Stagger:
                Stagger stagger = new Stagger(ccData.duration, ccData.dir);
                GetComponent<MainController>()?.cc.ApplyCC(stagger);
                break;
            case CrowdControl.Type.Knockback:
                break;
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
#if UNITY_EDITOR
        print($"{gameObject.name} died.");
#endif

    }
}
