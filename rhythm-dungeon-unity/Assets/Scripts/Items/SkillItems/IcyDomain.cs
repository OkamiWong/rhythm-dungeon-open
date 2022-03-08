using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcyDomain : SkillItem
{
	public int freezeRounds = 2;

	public override void Apply()
	{
		if (Effecting) return;
		if (CooldownCount < CooldownRounds) return;
		Effecting = true;

		List<Stats> list = GetOtherSideStatsList();
		foreach (Stats s in list)
		{
			s.effects.Add(FrozenEffect.Create(freezeRounds, s));
		}
	}
}
