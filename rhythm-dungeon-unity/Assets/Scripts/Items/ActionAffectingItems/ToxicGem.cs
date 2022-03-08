using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicGem : ActionAffectingItem
{
    public int damageEachTime = 5;
	public int lastRounds = 5;

    private void Awake()
    {
        ItemType = Type.Attack;
    }

    public override void OnAdded(Stats owner)
    {
        owner.AfterHitFunctions.Add(Poison);
    }

    void Poison(Stats target)
    {
		target.effects.Add(PoisonedEffect.Create(lastRounds, target, damageEachTime));
    }
}
