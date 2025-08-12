using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CardSettings : MonoBehaviour
{
	[SerializeField] private Image _mage;
	[SerializeField] private Sprite[] _cards;

	public void SetData(int index)
	{
		_mage.sprite = _cards[index];
	}
}