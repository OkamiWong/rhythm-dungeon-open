using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonedEffect : Effect
{
	[HideInInspector]
	public int damageEachTime;

	public static PoisonedEffect Create(int totalLife, Stats holder, int _damageEachTime)
	{
        var prefab = Prefabs[typeof(PoisonedEffect)];
		var effect = Instantiate(prefab).GetComponent<PoisonedEffect>();
		effect.damageEachTime = _damageEachTime;
		effect.BaseInit(totalLife, holder);
		return effect;
	}

	public override void OnTakingEffect()
	{
		var newColor = holder.sprite.color;
		newColor.r /= 2f;
		newColor.b /= 2f;
		holder.sprite.color = newColor;
	}

	public override void OnEffecting()
	{
		//as this is a continuous damage, we don't want it clear charge
		//the first "false" is to disable the charge clear
		holder.TakingDamage(damageEachTime, false);
	}

	public override void OnLosingEffect()
	{
		var originalColor = holder.sprite.color;
		originalColor.r *= 2f;
		originalColor.b *= 2f;
		holder.sprite.color = originalColor;
		Destroy(gameObject);
	}
}
