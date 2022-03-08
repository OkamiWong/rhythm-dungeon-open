using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The base class of an item. 
public class Item : MonoBehaviour
{
    public Sprite icon;
    public string itemName, itemDescription;

	[HideInInspector]
	public Stats userStats;
}
