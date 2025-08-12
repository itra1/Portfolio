using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Network.Rest;
using System;
using UnityEngine.EventSystems;
using com.ootii.Geometry;
using DG.Tweening;

namespace it.Game.Panels
{
	public class ClickToShowPanel : MonoBehaviour
	{
		[SerializeField] private RectTransform _cardParent;
		[SerializeField] private GameCardUI _gameCardPrefab;

		private PoolList<GameCardUI> _pooler;
		private List<DistributionCard> _cards = new List<DistributionCard>();
		private List<GameCardUI> _instanceCards = new List<GameCardUI>();
		private List<ulong> _openCards = new List<ulong>();
		private Table _table;
		private ulong _distributionId;
		private Tween _delaySend;

		public void Clear()
		{
			_cards.Clear();
			_openCards.Clear();
			_instanceCards.Clear();
		}

		public void SetCardPanel(Table table, ulong distributionId,  List<DistributionCard> cards)
		{
			if (_cards.Count != 0) return;
			_distributionId = distributionId;

			_table = table;
			_cards = cards;

			Spawn();
		}

		private void Spawn()
		{

			if (_pooler == null)
				_pooler = new PoolList<GameCardUI>(_gameCardPrefab, _cardParent);

			_pooler.HideAll();

			for (int i = 0; i < _cards.Count; i++)
			{

				var dCard = _cards[i];

				GameCardUI inst = (GameCardUI)_pooler.GetItem();
				inst.ShineSpriteRenderer.gameObject.SetActive(false);
				inst.IsMini = true;
				inst.Init(dCard);
				inst.SetOpenState();

				CardClickHandler clickHandler = inst.GetOrAddComponent<CardClickHandler>();

				clickHandler.OnClick = () =>
				{
					if (_openCards.Contains(dCard.id))
						_openCards.Remove(dCard.id);
					else
						_openCards.Add(dCard.id);
					ProcessOpencards();

					if (_delaySend != null && _delaySend.IsActive())
						_delaySend.Kill();

					_delaySend = DOVirtual.DelayedCall(0.5f, () =>
					{
						ClickToShow(inst, dCard);
					});
				};

				_instanceCards.Add(inst);

			}

			var _thisRect = transform.GetComponent<RectTransform>();
			_thisRect.sizeDelta = new Vector2(42 * _instanceCards.Count + Mathf.Abs(_cardParent.sizeDelta.x), _thisRect.sizeDelta.y);
#if !UNITY_STANDALONE
			_thisRect.sizeDelta = new Vector2(110 * _instanceCards.Count + Mathf.Abs(_cardParent.sizeDelta.x), _thisRect.sizeDelta.y);
#endif

			ProcessOpencards();

		}

		public void ClickToShow(GameCardUI card, DistributionCard dCard)
		{
			TableApi.ShowFoldCard(_table.id, _distributionId, _openCards, (result) =>
			{
				ProcessOpencards();
			});
		}

		private void ProcessOpencards()
		{
			foreach (var inst in _instanceCards)
			{
				CanvasGroup cg = inst.GetComponent<CanvasGroup>();
				if (_openCards.Contains(inst.Card.id))
				{
					cg.alpha = 1;
				}
				else
				{
					cg.alpha = 0.3f;
				}
			}
		}

	}

	public class CardClickHandler : MonoBehaviour, IPointerClickHandler
	{
		public Action OnClick;

		private void Awake()
		{
			Image img = gameObject.GetOrAddComponent<Image>();
			img.color = Color.clear;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick?.Invoke();
		}
	}

}