using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : Effect
{
	[HideInInspector]
	public int damageEachTime;

	public static BurnEffect Create(int totalLife, Stats holder, int _damageEachTime)
	{
        var prefab = Prefabs[typeof(BurnEffect)];
		var effect = Instantiate(prefab).GetComponent<BurnEffect>();
		effect.damageEachTime = _damageEachTime;
		effect.BaseInit(totalLife, holder);
		return effect;
	}

	public override void OnTakingEffect()
	{
		var newColor = holder.sprite.color;
		newColor.g /= 2f;
		newColor.b /= 2f;
		holder.sprite.color = newColor;
	}

	public override void OnEffecting()
	{
		//as this is a continuous damage, we don't want it clear charge
		//the second "false" is to disable the charge clear
		holder.TakingDamage(damageEachTime, false);
	}

	public override void OnLosingEffect()
	{
		var originalColor = holder.sprite.color;
		originalColor.g *= 2f;
		originalColor.b *= 2f;
		holder.sprite.color = originalColor;
		Destroy(gameObject);
	}
}

