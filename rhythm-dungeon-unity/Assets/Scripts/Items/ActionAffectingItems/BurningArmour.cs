using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningArmour : ActionAffectingItem
{
	public int damageEachTime = 10;
	public int lastRounds = 2;

	private void Awake()
	{
		ItemType = Type.Attack;
	}

	public override void OnAdded(Stats owner)
	{
		owner.AfterHittedFunctions.Add(ShooterOnFire);
	}

	public void ShooterOnFire(Stats shooter)
	{
		shooter.effects.Add(BurnEffect.Create(lastRounds, shooter, damageEachTime));
	}
}
