using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merciless : ActionAffectingItem
{
	[Range(0f, 1f)]
	public float killPercent = 0.2f;

	private void Awake()
	{
		ItemType = Type.Attack;		
	}

	public override void OnAdded(Stats owner)
	{
		int cnt = 1; //how many Merciless item we have
		foreach (Item item in owner.ActionAffectingItems)
		{
			if (item.name == name && item != this) cnt++;
		}
		float delta = killPercent; killPercent = 0;
		while (cnt-- > 0) { killPercent += delta; delta /= 2f; }
		owner.AfterHitFunctions.Add(MercilessKill);
	}

	void MercilessKill(Stats target)
	{
		if ((float)target.CurrentHealth / (float)target.MaxHealth < killPercent)
		{
			target.TakingDamage(target.CurrentHealth);
		}
	}
}
