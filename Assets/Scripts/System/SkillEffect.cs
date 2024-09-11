using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DamageData
{
    [Serializable]
    public struct Modifier
    {
        public enum Source
        {
            User_ATK,

        }
        public enum Type
        {
            Add,
            Mul,
        }

        public Source source;
        public Type type;
    }

    public float baseValue;

    public List<Modifier> modifiers;
}

public class SkillEffect : MonoBehaviour
{
    public Skill skill { get; protected set; }

    AudioSource audioSource;
    public AudioClip defaultClip;
    public AudioClip[] hitClips;

    protected Vector3 lastPosition;
    public Vector3 dir
    { 
        get
        {
            if (Vector3.Distance(transform.position, lastPosition) < 0.01f) return Vector3.zero;
            else return transform.position - lastPosition;
        }
    }

    public DamageData damageData;

    public GameObject owner { get; protected set; }

    protected float predelay;
    protected float duration;

    protected bool isOn;

    protected List<GameObject> alreadys = new List<GameObject>();

    public CrowdControlData ccData;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultClip;
        audioSource.Play();
    }

    protected virtual void Update()
    {
        if (!isOn) return;

        if (predelay > 0)
        {
            predelay -= Time.deltaTime;
        }

        if (predelay <= 0)
        {
            GetComponent<Collider>().enabled = true;
        }
    }

    protected virtual void LateUpdate()
    {
        if (!isOn) return;
        if (predelay > 0) return;

        if (duration > 0)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        lastPosition = transform.position;
    }

    public virtual void Init(Skill skill, GameObject owner)
    {
        this.skill = skill;
        this.owner = owner;
        predelay = skill.predelay;
        duration = skill.duration;
        GetComponent<Collider>().enabled = false;
        alreadys.Clear();

        isOn = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;

        if (alreadys.Contains(target)) return;
        alreadys.Add(target);

        if (target.layer != LayerMask.NameToLayer("Enemy")) return;

        if (!target.TryGetComponent(out Stats stats)) return;

        stats.Damaged(damageData.baseValue, ccData); //TODO: mod Àû¿ë

        //main.cc.ApplyCC(stagger);

        //int clipIdx = UnityEngine.Random.Range(0, hitClips.Length);
        //
        //audioSource.clip = hitClips[clipIdx];
        //audioSource.Play();
    }
}
