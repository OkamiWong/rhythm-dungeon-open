using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectGenerator : MonoBehaviour
{
	public GameObject[] items;
	public float biggestX, biggestY;
	public int numberOfItems;
	private void Start()
	{
		for(int i = 0; i < numberOfItems; ++i)
		{
			var item = Instantiate(items[UnityEngine.Random.Range(0, items.Length)]);
			item.transform.position = new Vector3(
				UnityEngine.Random.Range(-biggestX, biggestX) + transform.position.x,
				UnityEngine.Random.Range(-biggestY, biggestY) + transform.position.y,
				0f);
			item.transform.SetParent(gameObject.transform);
		}
	}
}
