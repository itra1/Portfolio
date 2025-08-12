using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FiltersSwitch : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.FilterSwitchButtonUI[] FilterButtons;
	[SerializeField] private GameObject[] Labels;

	private void Start()
	{
		ChangeFilter(0);
	}

	public void ChangeFilter(int ChoosenButton)
	{

		for (int a = 0; a < FilterButtons.Length; a++)
		{
			FilterButtons[a].interactable = true;
			Labels[a * 2].SetActive(false);
			Labels[a * 2 + 1].SetActive(true);
			var im = FilterButtons[a].GetComponent<Image>();
			var cl = im.color;
			cl.a = 0;
			im.color = cl;
		}
		var image = FilterButtons[ChoosenButton].GetComponent<Image>();
		var c = image.color;
		c.a = 1;
		image.color = c;
		FilterButtons[ChoosenButton].interactable = false;
		Labels[ChoosenButton * 2].SetActive(true);
		Labels[ChoosenButton * 2 + 1].SetActive(false);
		/*#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
					TablesUIManager.Inst.TableDisable(); //nessesarry to avoid table deletion
		#endif*/
	}

	public void OnEnable()
	{
		ChangeFilter(0);
	}
}
