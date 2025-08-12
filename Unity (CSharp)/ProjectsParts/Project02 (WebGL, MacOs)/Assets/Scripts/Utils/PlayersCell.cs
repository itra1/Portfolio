using it;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Garilla;

public class PlayersCell : MonoBehaviour
{
	[SerializeField] private RectTransform _rect;
	[SerializeField] private Bubushka _cellPrefab;

	private List<Bubushka> _cells = new List<Bubushka>();

	public void SetData(int count, int active)
	{
		_cells.ForEach(x => x.gameObject.SetActive(false));
		_cellPrefab.gameObject.SetActive(false);
		for (int i = 0; i < count; i++)
		{

			Bubushka c = _cells.Find(x => !x.gameObject.activeInHierarchy);

			if (c == null)
			{
				GameObject inst = Instantiate(_cellPrefab.gameObject, _cellPrefab.transform.parent);
				c = inst.GetComponent<Bubushka>();
				_cells.Add(c);
			}
			c.gameObject.SetActive(true);
			c.FillImage.gameObject.SetActive(i < active);
		}
		_rect.sizeDelta = new Vector2(count * 10.55f, _rect.sizeDelta.y);
	}


}
