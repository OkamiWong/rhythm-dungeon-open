using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenEffect : Effect
{
	public static FrozenEffect Create(int totalLife, Stats holder)
	{
        var prefab = Prefabs[typeof(FrozenEffect)];
		var effect = Instantiate(prefab).GetComponent<FrozenEffect>();
        if (holder.isBoss) effect.transform.localScale *= 2;
        effect.BaseInit(totalLife, holder);
		return effect;
	}

	public override void OnTakingEffect()
	{
		holder.Freeze();
	}

	public override void OnLosingEffect()
	{
		holder.Unfreeze();
		Destroy(gameObject);
	}
}
