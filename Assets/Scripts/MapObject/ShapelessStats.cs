using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class ShapelessStats : MonoBehaviour
{
    Stats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.hp <= 0)
        {
            transform.root.gameObject.SetActive(false);
        }
    }
}
