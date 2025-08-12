using System.Collections;
using System.Collections.Generic;
using Garilla.Games;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Garilla.BadBeat;
using TMPro;
using it.Network.Rest;
using System;

namespace it.Game.Panels
{
	public class BadBeatPanel : MonoBehaviour
	{
		[SerializeField] private RectTransform _content;
		[SerializeField] private RectTransform _cardParent;
		[SerializeField] private AnimationCurve _animCurve;
		[SerializeField] private TextMeshProUGUI _amountLabel;
		[SerializeField] private Image _cardPrefab;
		[SerializeField] private CardLibrary _cardLibrary;

		public GameUIManager GameManager { get; set; }
		private List<Image> _cards = new List<Image>();

		private void Awake()
		{
			_content.gameObject.SetActive(false);
		}

		private void Start()
		{
			BadBeatController.Instance.OnAward -= ShowBadBeat;
			BadBeatController.Instance.OnAward += ShowBadBeat;
		}

		private void OnDestroy()
		{
			BadBeatController.Instance.OnAward -= ShowBadBeat;
		}
		//[ContextMenu("ShowWin")]
		//public void TextWin(){
		//	ShowBadBeat(1354.64m);
		//}

		public void ShowBadBeat(BedBeatAwardData barBeatData)
		{
			if (barBeatData.UserId != UserController.User.id) return;
			if (barBeatData.TableId != GameManager.SelectTable.id) return;

			if(_cards.Count > 0){
				for (int i = 0; i < _cards.Count; i++)
					Destroy(_cards[i].gameObject);
				_cards.Clear();
			}


			decimal cell = System.Math.Floor(barBeatData.Award);
			decimal prop = barBeatData.Award - cell;
			_amountLabel.text = $"{StringConstants.CURRENCY_SYMBOL}{it.Helpers.Currency.String(cell, false)}.<size=50%>{System.Math.Floor(prop * 100)}";

			for (int i = 0; i < barBeatData.Cards.Length; i++)
			{
				_cardParent.gameObject.SetActive(true);
				var item = Instantiate(_cardPrefab, _cardParent);
				item.sprite = _cardLibrary.GetCard(barBeatData.Cards[i]);
				_cards.Add(item);
			}


			StartCoroutine(ShowWinn());
		}

		IEnumerator ShowWinn()
		{
			_content.localScale = Vector3.zero;
			_content.gameObject.SetActive(true);
			_content.DOScale(Vector3.one, 0.25f).SetEase(_animCurve);
			yield return new WaitForSeconds(3.5f);
			_content.DOScale(Vector3.zero, 0.25f);
			yield return new WaitForSeconds(0.25f);
			_content.gameObject.SetActive(false);

		}

	}
}