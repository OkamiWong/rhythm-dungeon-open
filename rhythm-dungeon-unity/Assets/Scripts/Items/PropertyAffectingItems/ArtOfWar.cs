using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtOfWar : PropertyAffectingItem
{
	public float deltaChargeFactor = 0.25f;

	public override float CalculateDeltaChargeFactor()
	{
		return deltaChargeFactor;
	}
}
