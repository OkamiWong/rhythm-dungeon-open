using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermellon : SkillItem
{

	public override void Apply()
	{
		if (Effecting) return;
		if (CooldownCount < CooldownRounds) return;
		Effecting = true;

		userStats.AddingHealth(userStats.Strength / 2);
	}
}
