using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LuckyRing : PropertyAffectingItem
{
    public int luckIncreaseAmount = 50;

    public override int CalculateDeltaLuck()
    {
        return luckIncreaseAmount;
    }
}
