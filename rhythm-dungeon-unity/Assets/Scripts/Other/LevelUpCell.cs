using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used in levelup panel
public class LevelUpCell : MonoBehaviour
{
	public GameObject icon, title, description;

	public void MouseEnter()
	{
		icon.SetActive(false);
		title.SetActive(false);
		description.SetActive(true);
	}

	public void MouseExit()
	{
		icon.SetActive(true);
		title.SetActive(true);
		description.SetActive(false);
	}
}
