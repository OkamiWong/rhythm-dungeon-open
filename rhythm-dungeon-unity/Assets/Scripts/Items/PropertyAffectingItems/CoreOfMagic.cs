using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreOfMagic : PropertyAffectingItem
{
    public override int CalculateDeltaCooldown()
    {
        return -1;
    }
}
