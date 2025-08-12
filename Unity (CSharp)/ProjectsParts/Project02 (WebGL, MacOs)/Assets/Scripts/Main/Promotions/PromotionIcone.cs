using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using it.Network.Rest;
using System.Drawing;
using System.Linq;

namespace Garilla.Promotions
{
	public class PromotionIcone : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnCompleteVisible;

		[SerializeField] private PromotionInfoCategory _type;
		[SerializeField] private List<GameType> _gameTypes;
		[SerializeField] private bool _isVipGame;
		[SerializeField] private bool _isAllOrNothing;
		[SerializeField] private bool _isClicked = true;

		private RectTransform _content;
		private Animator _animator;
		private bool _isAnimateWin;
		private bool _isIncrementAnim;
		private Table _table;

		public PromotionInfoCategory Type { get => _type; set => _type = value; }

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			_content = _animator.GetComponent<RectTransform>();


			GetComponent<it.UI.Elements.GraphicButtonUI>().OnClick.AddListener(() =>
			{
				if (!_isClicked) return;
				OnIncrement(_type, (int)PromotionController.Instance.GetCounter(_type, _table.GetCategory()));
			});

		}

		public void SetTable(Table table)
		{
			_table = table;
		}

		public bool IsActual(Table table)
		{
			if (_isVipGame)
				return table.is_vip;
			//if (_isAllOrNothing)
			//	return table.is_all_or_nothing;

			return table.is_all_or_nothing == _isAllOrNothing || (!table.is_all_or_nothing && _gameTypes.Contains((GameType)table.game_rule_id));

		}

		//private void Start()
		//{
		//	PromotionController.Instance.OnAward -= OnAward;
		//	PromotionController.Instance.OnAward += OnAward;
		//}

		//private void OnDestroy()
		//{
		//	PromotionController.Instance.OnAward -= OnAward;
		//}

		public void OnAward(PromotionInfoCategory type, int max, decimal value)
		{
			if (_isIncrementAnim)
			{
				_isIncrementAnim = false;
				_isAnimateWin = false;
				StopAllCoroutines();
			}

			if (!_isAnimateWin)
				StopAllCoroutines();

			if (!IsActual(_table))
			{
				OnCompleteVisible?.Invoke();
				return;
			}

			StartCoroutine(AnimCoroutine(type, max, value));
		}

		public void OnIncrement(PromotionInfoCategory type, int value)
		{
			if (!_isAnimateWin)
				StopAllCoroutines();

			if (!IsActual(_table))
			{
				OnCompleteVisible?.Invoke();
				return;
			}
			StartCoroutine(AnimIncrementCoroutine(type, value));
		}
		IEnumerator AnimIncrementCoroutine(PromotionInfoCategory type, int value)
		{
			if (_isAnimateWin) yield break;
			_isIncrementAnim = true;
			_animator.gameObject.SetActive(false);
			_animator.gameObject.SetActive(true);
			_animator.SetTrigger("hide");

			bool isMe = type == _type;

			if (!isMe)
			{
				yield break;
			}

			_isAnimateWin = true;

			yield return new WaitForSeconds(1.2f);

			_content.gameObject.SetActive(false);

			var go = ResourceManager.GetResource<GameObject>("Prefabs/Promotions/ChangeItem");

			var inst = Instantiate(go, transform);
			inst.transform.localScale = Vector3.one;
			inst.transform.localPosition = Vector3.zero;
			var instRect = inst.GetComponent<RectTransform>();
			instRect.anchorMin = Vector2.zero;
			instRect.anchorMax = Vector2.one;
			instRect.anchoredPosition = Vector3.zero;
			instRect.sizeDelta = Vector2.zero;
			PromotionIconeIncrement inc = inst.GetComponent<PromotionIconeIncrement>();
			inc.SetData(value, -1);
			inst.gameObject.SetActive(true);
			yield return inc.Show();

			_content.gameObject.SetActive(true);
			_content.localScale = Vector3.zero;

			_content.DOScale(Vector3.one, 0.3f);
			_isIncrementAnim = false;
			_isAnimateWin = false;
			OnCompleteVisible?.Invoke();
		}

		IEnumerator AnimCoroutine(PromotionInfoCategory type, int max, decimal value, System.Action OnComplete = null)
		{
			if (_isAnimateWin) yield break;

			_animator.gameObject.SetActive(false);
			_animator.gameObject.SetActive(true);
			_animator.SetTrigger("hide");

			bool isMe = type == _type;

			if (isMe)
				_isAnimateWin = true;

			yield return new WaitForSeconds(1.2f);

			_content.gameObject.SetActive(false);

			var go = ResourceManager.GetResource<GameObject>("Prefabs/Promotions/ChangeItem");

			if (isMe)
			{
				var inst = Instantiate(go, transform);
				inst.transform.localScale = Vector3.one;
				inst.transform.localPosition = Vector3.zero;
				var instRect = inst.GetComponent<RectTransform>();
				instRect.anchorMin = Vector2.zero;
				instRect.anchorMax = Vector2.one;
				instRect.anchoredPosition = Vector3.zero;
				instRect.sizeDelta = Vector2.zero;
				PromotionIconeIncrement inc = inst.GetComponent<PromotionIconeIncrement>();
				inc.SetData(max, value);
				inst.gameObject.SetActive(true);
				yield return inc.Show();
			}

			if (!isMe)
			{
				yield return new WaitForSeconds(2.5f);
			}
			_content.gameObject.SetActive(true);
			_content.localScale = Vector3.zero;

			_content.DOScale(Vector3.one, 0.3f);
			_isAnimateWin = false;
			OnCompleteVisible?.Invoke();
		}

	}
}