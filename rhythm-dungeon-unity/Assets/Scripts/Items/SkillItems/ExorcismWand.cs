using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExorcismWand : SkillItem
{
	public override void Apply()
	{
		if (Effecting) return;
		if (CooldownCount < CooldownRounds) return;
		Effecting = true;

		foreach (Effect e in userStats.effects)
			e.OnLosingEffect();
		userStats.effects.Clear();
	}
}
