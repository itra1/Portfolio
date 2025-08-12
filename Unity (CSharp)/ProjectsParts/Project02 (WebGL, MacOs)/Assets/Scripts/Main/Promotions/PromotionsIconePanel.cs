using it.Network.Rest;

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using UnityEngine;

namespace Garilla.Promotions
{
	public class PromotionsIconePanel : MonoBehaviour
	{
		private List<PromotionIcone> _icons;
		private Garilla.Games.GameUIManager _manager;
		private bool _isAnimate;
		private Queue<AnimVisibleStruct> _visibleQueue = new Queue<AnimVisibleStruct>();

		[SerializeField] private bool _singleIcone = true;
		[SerializeField] private bool _isBalanceBanel = false;

		public Garilla.Games.GameUIManager GameManager
		{
			get
			{
				return _manager;
			}
			set
			{
				_manager = value;
#if UNITY_STANDALONE
				_icons.ForEach(x => x.gameObject.SetActive(x.IsActual(_manager.SelectTable)));
#else
				if (_singleIcone)
					_icons.ForEach(x => x.gameObject.SetActive(false));
				else
					_icons.ForEach(x => x.gameObject.SetActive(x.IsActual(_manager.SelectTable)));
#endif
			}
		}

		public void SelectTable(it.Network.Rest.Table t)
		{
			if (_icons.Count == 0)
				_icons = GetComponentsInChildren<PromotionIcone>(true).ToList();

			_icons.ForEach(x => x.SetTable(t));
		}

		private struct AnimVisibleStruct
		{
			public PromotionInfoCategory Type;
			public int Max;
			public decimal Value;
			public bool IsAward;
		}

		private void Awake()
		{
			_icons = GetComponentsInChildren<PromotionIcone>(true).ToList();

		}

		private void ShowCounter()
		{

		}

		private void Start()
		{
			//PromotionController.Instance.OnAward -= OnAward;
			//PromotionController.Instance.OnAward += OnAward;
			//PromotionController.Instance.OnIncrement -= OnIncrement;
			//PromotionController.Instance.OnIncrement += OnIncrement;
			PromotionController.Instance.OnPromotion -= OnPromotions;
			PromotionController.Instance.OnPromotion += OnPromotions;
		}

		private void OnDestroy()
		{
			//PromotionController.Instance.OnAward -= OnAward;
			//PromotionController.Instance.OnIncrement -= OnIncrement;
			PromotionController.Instance.OnPromotion -= OnPromotions;
		}
		private void OnPromotions(PromotionEventData eventData)
		{
			if (eventData.IsIncrement)
			{
				OnIncrement(eventData);
			}
			else
			{
				OnAward(eventData);
			}

		}

		private void OnAward(PromotionEventData eventData)
		{
			//if (GameManager.SelectTable.GetCategory() != tableType) return;
			if (GameManager.SelectTable.id != eventData.TableId) return;
			if (eventData.UserId != UserController.User.id) return;
#if UNITY_STANDALONE
			_icons.ForEach(x => x.OnAward(eventData.Category, eventData.Limit, (int)eventData.Value));
#else
			if (_singleIcone)
			{
				_visibleQueue.Enqueue(new AnimVisibleStruct { Type = eventData.Category, Max = eventData.Limit, Value = eventData.Value, IsAward = true });
				VisibleEnement();
			}
			else
			{
				_icons.ForEach(x => x.OnAward(eventData.Category, eventData.Limit, (int)eventData.Value));
			}
#endif
		}

		private void OnIncrement(PromotionEventData eventData)
		{
			//if (GameManager.SelectTable.GetCategory() != tableType) return;
			//if (eventData.UserId != UserController.User.id) return;
			if (GameManager.SelectTable.id != eventData.TableId) return;

			for (int i = 0; i < _icons.Count; i++)
			{
				if (_icons[i].Type == eventData.Category)
				{
					if (_singleIcone)
					{
						_visibleQueue.Enqueue(new AnimVisibleStruct { Type = eventData.Category, Max = -1, Value = eventData.Value, IsAward = false });
						VisibleEnement();
					}
					else
					{
						_icons[i].OnIncrement(eventData.Category, (int)eventData.Value);
					}
				}
			}
		}

		private void VisibleEnement()
		{

			if (_isAnimate) return;

			if (_visibleQueue.Count == 0) return;

			var itm = _visibleQueue.Dequeue();

			var icon = _icons.Find(x => x.Type == itm.Type);

			if (!icon.IsActual(GameManager.SelectTable))
			{
				VisibleEnement();
				return;
			}

			icon.gameObject.SetActive(true);
			if (itm.IsAward)
			{
				icon.OnAward(itm.Type, itm.Max, itm.Value);
			}
			else
			{
				icon.OnIncrement(itm.Type, (int)itm.Value);
			}
			_isAnimate = true;
			icon.OnCompleteVisible = () =>
			{
				if (_singleIcone)
					icon.gameObject.SetActive(false);
				_isAnimate = false;
				VisibleEnement();
			};

		}

	}
}