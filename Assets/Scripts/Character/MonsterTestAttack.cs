using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTestAttack : MonoBehaviour
{
    List<GameObject> alreadys = new List<GameObject>();

    public float damage;
    public CrowdControlData data;

    float elapsed;

    private void OnEnable()
    {
        alreadys.Clear();
        elapsed = 0;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 0.2f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        if (alreadys.Contains(target)) return;
        alreadys.Add(target);

        if (!target.TryGetComponent(out Stats stats)) return;

        stats.Damaged(damage, data);
    }
}
