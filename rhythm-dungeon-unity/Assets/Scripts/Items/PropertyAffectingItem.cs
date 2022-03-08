using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The base class of the item that affects a character's property steadily.
//By default, there is no influence at all. 
public class PropertyAffectingItem : Item
{
    public virtual int CalculateDeltaMaxHealth()
    {
        return 0;
    }

    public virtual int CalculateDeltaStrength()
    {
        return 0;
    }

    public virtual int CalculateDeltaLuck()
    {
        return 0;
    }

	public virtual float CalculateDeltaChargeFactor()
	{
		return 0;
	}

	public virtual int CalculateDeltaCooldown()
	{
		return 0;
	}
}
