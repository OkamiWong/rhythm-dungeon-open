using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Destroys every background item that overlaps with it
public class BackgroundObjectCleaner : MonoBehaviour
{
	private void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.tag == "BackgroundItem")
			Destroy(collider.gameObject);
	}
}
