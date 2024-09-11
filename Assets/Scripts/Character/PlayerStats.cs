using System.Collections.Generic;
using UnityEngine;
using Morph = PlayerMainController.Morph;

public class PlayerStats : Stats, IUpdater
{
    PlayerMainController playerMain;

    Dictionary<Morph, int> morphPoints = new();

    public int currentMorphPoint = 100;
    public float morphDuration;
    const float MAX_MORPH_DURATION = 10f;

    bool isMorphed;

    protected override void Start()
    {
        base.Start();

        playerMain = GetComponent<PlayerMainController>();

        morphPoints.Add(Morph.BACK, 1);
        morphPoints.Add(Morph.LEFT_HAND, 2);
        morphPoints.Add(Morph.RIGHT_HAND, 2);
        morphPoints.Add(Morph.LEGS, 3);
    }

    void IUpdater.Update()
    {
        if (isMorphed)
        {
            morphDuration -= Time.deltaTime;

            if (morphDuration <= 0)
            {
                isMorphed = false;
                playerMain.DoMorph(Morph.NONE);
            }
        }
    }

    public bool GetMorph(Morph morph)
    {
        int cost = 0;

        foreach (Morph key in morphPoints.Keys)
        {
            if ((morph & key) != 0)
            {
                cost += morphPoints[key];
            }
        }

        if (currentMorphPoint >= cost)
        {
            currentMorphPoint -= cost;
            morphDuration = MAX_MORPH_DURATION;
            isMorphed = true;

            return true;
        }
        else
        {
            return false;
        }
    }
}
