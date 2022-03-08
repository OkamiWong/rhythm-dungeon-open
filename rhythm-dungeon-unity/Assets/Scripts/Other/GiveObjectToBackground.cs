using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveObjectToBackground : MonoBehaviour
{
	public GameObject[] objectToGive;

	private void Start()
	{
		var background = GameObject.Find("Background");
		foreach(GameObject obj in objectToGive)
		{
			obj.transform.SetParent(background.transform);
		}
	}
}
