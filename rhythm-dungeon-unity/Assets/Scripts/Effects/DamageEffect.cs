using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect
{
	private float lifeTime = 0.2f;

	public IEnumerator OnDamage(SpriteRenderer sprite)
	{
		var c = sprite.color;
		c.g /= 3; c.b /= 3;
		sprite.color = c;

		yield return new WaitForSeconds(lifeTime);

		c = sprite.color;
		c.g *= 3; c.b *= 3;
		sprite.color = c;
	}
}
