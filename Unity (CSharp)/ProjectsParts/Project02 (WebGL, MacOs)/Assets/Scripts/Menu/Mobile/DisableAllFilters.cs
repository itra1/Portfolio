using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisableAllFilters : MonoBehaviour
{

	[SerializeField] private GameObject[] allFilters;
	[SerializeField] private Transform ViewportToStore;

	public void DisableFilterObjects()
	{
		for (int a = 0; a < allFilters.Length; a++)
		{
			allFilters[a].SetActive(false);
			allFilters[a].transform.SetParent(ViewportToStore);
		}

	}
}
