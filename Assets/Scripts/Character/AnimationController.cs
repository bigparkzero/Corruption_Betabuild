using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationController
{
    Animator anim;
    Dictionary<string, int> animIDs = new Dictionary<string, int>();
    float currentProgress;
    float lastStopTime;
    public bool isAnimationStopped;

    public AnimationController(Animator anim)
    {
        this.anim = anim;
    }

    public void SetAnimID(string id)
    {
        if (!animIDs.ContainsKey(id))
        {
            animIDs.Add(id, Animator.StringToHash(id));
        }
    }

    public void SetFloat(string name, float value)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(name))
        {
            anim.SetFloat(animIDs[name], value);
        }
        else
        {
            animIDs.Add(name, Animator.StringToHash(name));
            anim.SetFloat(animIDs[name], value);
        }
    }

    public void SetBool(string name, bool value)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(name))
        {
            anim.SetBool(animIDs[name], value);
        }
        else
        {
            animIDs.Add(name, Animator.StringToHash(name));
            anim.SetBool(animIDs[name], value);
        }
    }

    public void SetInteger(string name, int value)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(name))
        {
            anim.SetInteger(animIDs[name], value);
        }
        else
        {
            animIDs.Add(name, Animator.StringToHash(name));
            anim.SetInteger(animIDs[name], value);
        }
    }

    public void SetTrigger(string name)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(name))
        {
            anim.SetTrigger(animIDs[name]);
        }
        else
        {
            animIDs.Add(name, Animator.StringToHash(name));
            anim.SetTrigger(animIDs[name]);
        }
    }

    public void ResetTrigger(string name)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(name))
        {
            anim.ResetTrigger(animIDs[name]);
        }
        else
        {
            animIDs.Add(name, Animator.StringToHash(name));
            anim.ResetTrigger(animIDs[name]);
        }
    }

    public void StopMotion()
    {
        if (isAnimationStopped) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentProgress = stateInfo.normalizedTime % 1.0f;
        lastStopTime = Time.time;

        anim.speed = 0;
        isAnimationStopped = true;
    }

    public void ResumeMotion()
    {
        if (lastStopTime == 0) return;

        float elapsedTime = Time.time - lastStopTime;

        anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, currentProgress + elapsedTime);
        anim.speed = 1;

        isAnimationStopped = false;
    }

    void ResetAnimationSpeedOnTransition()
    {
        if (anim.IsInTransition(0))
        {
            anim.speed = 1;
            isAnimationStopped = false;
        }
    }

    public AnimatorStateInfo GetCurrentState()
    {
        return anim.GetCurrentAnimatorStateInfo(0);
    }
    public AnimatorStateInfo GetNextState()
    {
        return anim.GetNextAnimatorStateInfo(0);
    }

    public void CrossFade(string stateName, float normalizedTransitionDuration)
    {
        ResetAnimationSpeedOnTransition();

        if (animIDs.ContainsKey(stateName))
        {
            anim.CrossFade(animIDs[stateName], normalizedTransitionDuration);
        }
        else
        {
            animIDs.Add(stateName, Animator.StringToHash(stateName));
            anim.CrossFade(animIDs[stateName], normalizedTransitionDuration);
        }
    }

    public void ChangeAnimator(Animator newAnim)
    {
        var info = anim.GetCurrentAnimatorStateInfo(0);
        anim = newAnim;
        anim.Play(info.fullPathHash, 0, info.normalizedTime);
    }
}
