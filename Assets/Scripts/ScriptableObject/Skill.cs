using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public enum Type
    {
        Melee,
        Projectile,
        AOE,
        // special, heal, hold ..
    }

    [SerializeField] int _id;
    [SerializeField] string _skill_name;
    [SerializeField] Type _type;
    [SerializeField] DamageData _damage;
    [SerializeField] GameObject _prefab_effect;
    [SerializeField] int _effect_position_index;
    [SerializeField] Quaternion _rotation;
    [SerializeField] float _predelay;
    [SerializeField] float _duration;
    [SerializeField] float _cooldown;
    [SerializeField] GameObject[] _prefab_additionals;
    [SerializeField] Skill[] _chains;

    public int id { get { return _id; } }
    public string skill_name { get { return _skill_name; } }

    public Type type { get { return _type; } }
    public DamageData damage { get { return _damage; } }
    public GameObject prefab_effect { get { return _prefab_effect; } }
    public int effect_position_index { get { return _effect_position_index; } }
    public Quaternion rotation { get { return _rotation; } }
    public float predelay { get { return _predelay; } }
    public float duration { get { return _duration; } }
    public float cooldown { get { return _cooldown; } }
    public GameObject[] prefab_additionals { get { return _prefab_additionals; } }
    public Skill[] chains { get { return _chains; } }
}
