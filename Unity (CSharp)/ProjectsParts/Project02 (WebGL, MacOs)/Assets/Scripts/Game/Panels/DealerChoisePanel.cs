using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace it.Game.Panels
{
	public class DealerChoisePanel : MonoBehaviour
	{
		[SerializeField] private Image _image;
		[SerializeField] private Sprite _defaultSprite;
		[SerializeField] private List<ButtonData> _data;

		[System.Serializable]
		private class ButtonData
		{
			public GameType GameType;
			public Sprite Sprite;
		}
		private void Awake()
		{
			//_image.gameObject.SetActive(false);
			_image.sprite = _defaultSprite;
		}

		public void SetTable(it.Network.Rest.Table table)
		{
			Set(table.game_rule_id);
		}
		public void Set(int ruleId)
		{
			GameType target = (GameType)ruleId;
			it.Logger.Log("GameType " + target.ToString());
			var data = _data.Find(x => x.GameType == target);
			if (data != null)
			{
				_image.sprite = data.Sprite;
				//_image.gameObject.SetActive(true);
			}
			else
			{
				_image.sprite = _defaultSprite;
				//_image.gameObject.SetActive(false);
			}

		}
	}
}