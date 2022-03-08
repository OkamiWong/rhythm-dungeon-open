using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCharacterStats : EnemyStats
{
    //The properties are increased accroding to
    //the number of bosses that the player has beaten.
    protected override void InitializeProperty()
    {
        float enhanceFactor = Mathf.Log(2f + 3f * GameManager.Instance.level, 2f);
        rawMaxHealth = (int)(rawMaxHealth * ((enhanceFactor - 1f) * 1.5f + 1f));
        rawStrength = (int)(rawStrength * ((enhanceFactor - 1f) * 0.75f + 1f));
        rawLuck = rawLuck + GameManager.Instance.level * 10;

        isPropertyInitialized = true;
    }
}
