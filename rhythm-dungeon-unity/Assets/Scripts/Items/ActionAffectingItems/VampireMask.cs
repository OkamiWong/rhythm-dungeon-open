using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireMask : ActionAffectingItem
{
	public float recoveryRate = 0.2f;

	void Awake()
	{
		ItemType = Type.Attack;
	}

	public override void OnAdded(Stats owner)
	{
		owner.AfterHitFunctions.Add(Bloodsucking);
	}

	void Bloodsucking(Stats target)
	{
        var recoveryAmount = target.lastDamage * recoveryRate;
        userStats.AddingHealth(recoveryAmount);
	}
}
