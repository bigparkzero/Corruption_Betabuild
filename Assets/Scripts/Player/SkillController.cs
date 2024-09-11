using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMainController;
using SkillDictionary = System.Collections.Generic.Dictionary<SkillController.SkillList, int>;

public class SkillController
{
    public enum SkillList
    {
        Basic_Ground,
        Basic_Air,
        Special_Ground,
        Special_Air,
    }

    PlayerMainController player;

    int currentSkillIdx = -1;
    SkillEffect currentEffect;

    int currentChainIndex = 0;

    Dictionary<Morph, SkillDictionary> skills = new Dictionary<Morph, SkillDictionary>();

    Dictionary<int, float> cooldowns = new Dictionary<int, float>(); //TODO: 쿨타임 등록하고 관리할 것.

    public SkillController(PlayerMainController player)
    {
        this.player = player;

        int maxMorphCombinations = (int)Math.Pow(2, Enum.GetValues(typeof(Morph)).Length);

        for (int i = 0; i < maxMorphCombinations; i++)
        {
            Morph morphCombination = (Morph)i;

            skills[morphCombination] = new SkillDictionary();

            switch (morphCombination)
            {
                case Morph.NONE:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    skills[morphCombination].Add(SkillList.Basic_Air, 1);
                    break;

                case Morph.LEFT_HAND:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    skills[morphCombination].Add(SkillList.Basic_Air, 1);
                    break;
                case Morph.RIGHT_HAND:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.BACK:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;

                case Morph.LEFT_HAND | Morph.RIGHT_HAND:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.LEFT_HAND | Morph.BACK:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.LEFT_HAND | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.RIGHT_HAND | Morph.BACK:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.RIGHT_HAND | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.BACK | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;

                case Morph.LEFT_HAND | Morph.RIGHT_HAND | Morph.BACK:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.LEFT_HAND | Morph.RIGHT_HAND | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.LEFT_HAND | Morph.BACK | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
                case Morph.RIGHT_HAND | Morph.BACK | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;

                case Morph.LEFT_HAND | Morph.RIGHT_HAND | Morph.BACK | Morph.LEGS:
                    skills[morphCombination].Add(SkillList.Basic_Ground, 0);
                    break;
            }
        }
    }

    // IUpdater로 업데이트 등록하고 실행.(쿨타임 처리)

    public bool DoSkill(KeyBind.Action input)
    {
        int skillIdx = GetSkillIndex(input);
        bool isSkillUsable = IsSkillUsable(skillIdx);

        if (isSkillUsable)
        {
            Skill skill = SkillDatabase.Instance.skills[skillIdx];

            if (currentSkillIdx == skillIdx)
            {
                if (currentEffect != null)
                {
                    UnityEngine.Object.Destroy(currentEffect.gameObject);
                }
                int length = skill.chains.Length;
                skill = skill.chains[currentChainIndex];
                currentChainIndex = (currentChainIndex + 1) % length;
            }
            else
            {
                currentChainIndex = 0;
            }

            GameObject prefab = skill.prefab_effect;

            currentEffect = UnityEngine.Object.Instantiate(prefab, player.skillPosition.position, player.transform.rotation).GetComponent<SkillEffect>();
            currentEffect.Init(skill, player.gameObject);
            if (currentEffect.skill.type == Skill.Type.Melee) currentEffect.transform.SetParent(player.skillPosition);
            currentSkillIdx = skillIdx;
        }

        return isSkillUsable;
    }

    int GetSkillIndex(KeyBind.Action input)
    {
        int trySkillIdx = -1;

        Morph currentMorph = player.currentMorph;
        SkillList currentSkill = SkillList.Basic_Ground;

        switch (input)
        {
            case KeyBind.Action.BasicSkill:
                if (player.IsGrounded())
                {
                    currentSkill = SkillList.Basic_Ground;
                }
                else
                {
                    currentSkill = SkillList.Basic_Air;
                }
                break;
            case KeyBind.Action.Special_Skill:
                if (player.IsGrounded())
                {
                    currentSkill = SkillList.Special_Ground;
                }
                else
                {
                    currentSkill = SkillList.Special_Air;
                }
                break;
            //case KeyBind.Action.Weapon_Two:
            //    break;
            //case KeyBind.Action.Weapon_Three:
            //    break;
            //case KeyBind.Action.Weapon_Four:
            //    break;
            default:
                throw new System.Exception($"Unexpected action input: {input}. This action is not supported.");
        }

        trySkillIdx = skills[currentMorph][currentSkill];

        if (trySkillIdx < 0) throw new System.Exception("Unexpected skillIdx: -1. Something wrong.");

        return trySkillIdx;
    }

    bool IsSkillUsable(int skillIdx)
    {
        //TODO: 마나, 쿨타임 체크하고 사용가능 여부 확인

        return true; // 임시로 성공 처리
    }

    public void ForceStopSkill()
    {
        currentSkillIdx = -1;
        currentChainIndex = 0;

        if (currentEffect != null)
        {
            UnityEngine.Object.Destroy(currentEffect.gameObject);
        }
    }
}
