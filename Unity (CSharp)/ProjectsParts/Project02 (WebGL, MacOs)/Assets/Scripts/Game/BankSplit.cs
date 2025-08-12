using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Garilla.Games
{

	/// <summary>
	/// Разделение банка
	/// </summary>
	public class BankSplit : MonoBehaviour
	{
		[SerializeField] private RectTransform _content;
		[SerializeField] private List<ContentStruct> _contents;
		[SerializeField] private RectTransform _betItem;
		[SerializeField] private List<RectTransform> _itemsPositions = new List<RectTransform>();

		public GameUIManager GameManager { get => _gameManager; set => _gameManager = value; }

		private GameUIManager _gameManager;
		private PoolList<RectTransform> _itemsPool;
		private List<RectTransform> _bankItems = new List<RectTransform>();

		[System.Serializable]
		public struct ContentStruct
		{
			public RectTransform Content;
			public int Max;
		}

		private void Awake()
		{
			_betItem.gameObject.SetActive(false);
			if (_itemsPool == null)
				_itemsPool = new PoolList<RectTransform>(_betItem.gameObject, _content);

#if !UNITY_STANDALONE
			//_betItem.localScale = Vector2.one * 2;
#endif

		}

		public void Clear()
		{
			if (_itemsPool == null)
				_itemsPool = new PoolList<RectTransform>(_betItem.gameObject, _content);

			_itemsPool.HideAll();
			_bankItems.Clear();
		}

		public void TableUpdate()
		{
			var sd = GameManager._currentSharedData;

			// Финишь? удялем
			if (sd.IsFinish || (sd.banks == null /*|| sd.Banks.Length <= 1*/ || !sd.ExistsFlop()))
			{
				Clear();
			}

			if (sd.banks == null /*|| sd.Banks.Length <= 1*/ || !sd.ExistsFlop()) return;

			//int bankCount = 0;

			//for (int i = sd.Banks.Count-1; i >= 0; i--)
			//{
			//	if (sd.Banks[i].amount <= 0) continue;
			//	bankCount++;
			//}

			// Добавляем недостающие банки
			//if (_bankItems.Count != bankCount)
			//{

			//	while (_bankItems.Count < bankCount)
			//	{
			//		var item = _itemsPool.GetItem();
			//		item.GetComponentInChildren<it.UI.Elements.Bets>().SetVisibleAnimate();
			//		_bankItems.Add(item);
			//	}

			//}
			//_content.sizeDelta = new Vector2(67 * _bankItems.Count, _content.sizeDelta.y);

			float width = 0;
			int index = -1;

			// Устанавливаем значение
			for (int i = sd.banks.Count - 1; i >= 0; i--)
			{
				decimal amount = _gameManager.GamePanel.GameSession.IsFullBank
					? sd.banks[i].amount
					: sd.banks[i].amount - sd.banks[i].stage_amount;

				if (amount <= 0.001m) continue;
				index++;

				if (_bankItems.Count < index+1)
				{
					var item = _itemsPool.GetItem();
					//item.GetComponentInChildren<it.UI.Elements.Bets>().SetVisibleAnimate();
					_bankItems.Add(item);
#if !UNITY_STANDALONE

					int curIndex = index;
					for (int y = 0; y < _contents.Count; y++)
					{
						curIndex -= _contents[y].Max;
						if(curIndex < -1)
						{
							item.SetParent(_contents[y].Content);
							break;
						}
					}

#endif
				}


				var bt = _bankItems[index].GetComponentInChildren<it.UI.Elements.Bets>();
				bt.SetValue(_gameManager.SelectTable, amount);

				width += bt.Width();
				width += 9;
			}
			//#if UNITY_STANDALONE
			_content.sizeDelta = new Vector2(width, _content.sizeDelta.y);
//#endif
			//DOTween.To(() => _content.sizeDelta, (x) => _content.sizeDelta = x,
			//new Vector2(width, _content.sizeDelta.y), 0.3f);

#if !UNITY_STANDALONE
			for (int y = 0; y < _contents.Count; y++)
			{
				var b = _contents[y].Content.GetComponentsInChildren<it.UI.Elements.Bets>();

				if (b.Length <= 0) continue;

				float w = 0;
				for(int i = 0; i < b.Length; i++)
				{

					w += b[i].Width()*2.5f;
					//w += b[i].Width()/2;
				}
				_contents[y].Content.sizeDelta = new Vector2(w, _contents[y].Content.sizeDelta.y);

			}
#endif

		}

	}
}