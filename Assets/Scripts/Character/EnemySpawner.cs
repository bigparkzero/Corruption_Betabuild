using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnData
    {
        public GameObject prefab;
        public Transform pos;
    }

    public SpawnData[] enemies;

    List<GameObject> members = new List<GameObject>();

    bool isInCombat;
    PlayerMainController target = null;

    public float ATTACK_RADIUS = 1.5f;

    public float ATTACK_COOLDOWN = 0.8f;

    public int attackerCount;

    private void OnEnable()
    {
        foreach (var enemy in enemies)
        {
            GameObject go = Instantiate(enemy.prefab, enemy.pos.position, Quaternion.identity);
            members.Add(go);

            go.GetComponentInChildren<MonsterMainController>().spawner = this;
        }
    }

    void Update()
    {
        bool allDead = true;
        foreach (var enemy in members)
        {
            if (enemy.GetComponentInChildren<EnemyStats>().hp > 0)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!isInCombat)
        {
            target = null;

            foreach (var enemy in members)
            {
                MonsterMainController main = enemy.GetComponentInChildren<MonsterMainController>();

                if (main.target != null)
                {
                    target = main.target;
                    isInCombat = true;
                    break;
                }
            }

            if (isInCombat)
            {
                foreach (var enemy in members)
                {
                    enemy.GetComponentInChildren<MonsterMainController>().target = target;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
#endif
}
