using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protector : MonoBehaviour
{
	[HideInInspector]
	public ProtectorDeployer item;

	[HideInInspector]
	public string selfTag;

	[HideInInspector]
	public int health = 1;

	private void Start()
	{
		selfTag = item.userStats.tag;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		//collided by weapon from other side
		if (collider.gameObject.tag == "Weapon" &&
			collider.GetComponent<RangeWeapon>().selfTag != selfTag)
		{
			health--; //kill health by 1
			if (health <= 0)
			{
				item.OnInvalid(); //created by item
			}	
		}
	}
}
