using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAffectingItem : Item
{
    public enum Type
	{
        Move, Attack, Charge, Defend
    }

    public Type ItemType { get; protected set; }

    public virtual void OnAdded(Stats owner) { }
}
