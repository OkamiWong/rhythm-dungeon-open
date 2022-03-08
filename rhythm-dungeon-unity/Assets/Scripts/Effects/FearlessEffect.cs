using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearlessEffect : Effect
{
	[HideInInspector]
	public int damageEachTime;
	[HideInInspector]
	public int deltaStrength;

	public static FearlessEffect Create(int totalLife, Stats holder, int _damageEachTime, int _deltaStrength)
	{
        var prefab = Prefabs[typeof(FearlessEffect)];
		var effect = Instantiate(prefab).GetComponent<FearlessEffect>();
		effect.damageEachTime = _damageEachTime;
		effect.deltaStrength = _deltaStrength;
		effect.BaseInit(totalLife, holder);
		return effect;
	}

	public override void OnTakingEffect()
	{
		var newColor = holder.sprite.color;
		newColor.g /= 2f;
		newColor.b /= 2f;
		holder.sprite.color = newColor;
		holder.rawStrength += deltaStrength;
	}

	public override void OnEffecting()
	{
		holder.TakingDamage(damageEachTime, false);
	}

	public override void OnLosingEffect()
	{
		var originalColor = holder.sprite.color;
		originalColor.g *= 2f;
		originalColor.b *= 2f;
		holder.sprite.color = originalColor;
		holder.rawStrength -= deltaStrength;
		Destroy(gameObject);
	}
}
