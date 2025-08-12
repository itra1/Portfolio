using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Elements
{
	public class GamePlayToolItem : MonoBehaviour
	{
		[SerializeField] private Image _icone;
		[SerializeField] private RectTransform _countIcone;
		[SerializeField] private TMP_Text _countLabel;

		public void SetData(string key, Sprite sprite, int count = -1)
		{
			_icone.sprite = sprite;
			if (count < 0)
			{
				_countIcone.gameObject.SetActive(false);
			}
			else
			{
				_countIcone.gameObject.SetActive(true);
				_countLabel.text = count.ToString();
			}
		}
	}
}
