using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Garilla.Promotions;
using Garilla.BadBeat;
using Garilla.Jackpot;

namespace Garilla.Games
{
	internal class PlayerGameWinIcones : MonoBehaviour
	{
		[SerializeField] private RectTransform _parentIcons;

		private PlayerGameIcone _baseController;
		private Queue<UnityEngine.Events.UnityAction> _visibleQueue = new Queue<UnityEngine.Events.UnityAction>();
		private bool _isVisibleIcone;
		private GameObject _prefabInst;
		private string _prefabName;
		private void Start()
		{
			_baseController = GetComponent<PlayerGameIcone>();
		}

		private void OnEnable()
		{
			PromotionController.Instance.OnPromotion -= OnPromotions;
			PromotionController.Instance.OnPromotion += OnPromotions;
			BadBeatController.Instance.OnAward -= OnBedBeatAward;
			BadBeatController.Instance.OnAward += OnBedBeatAward;
			JackpotController.Instance.OnAward -= JackpotAward;
			JackpotController.Instance.OnAward += JackpotAward;
		}

		private void OnDisable()
		{
			PromotionController.Instance.OnPromotion -= OnPromotions;
			BadBeatController.Instance.OnAward -= OnBedBeatAward;
			JackpotController.Instance.OnAward -= JackpotAward;
		}

		private void OnDestroy()
		{
		if(_prefabInst != null)
			PoolerManager.Return(_prefabName, _prefabInst);
		}

		private void VisibleNext(bool fromAnim = false)
		{

			if (fromAnim)
			{
				_isVisibleIcone = false;
			}

			if (_isVisibleIcone) return;

			if (_visibleQueue.Count <= 0) return;

			var func = _visibleQueue.Dequeue();
			_isVisibleIcone = true;

			func.Invoke();
		}

		#region Промоушен

		private void OnPromotions(PromotionEventData eventData)
		{
			Debug.Log("OnPromotions");
			if (_baseController == null || _baseController.UserId != eventData.UserId) return;
			if (_baseController.GameController == null || _baseController.GameController.SelectTable.id != eventData.TableId) return;

			if (!eventData.IsIncrement)
			{
				_visibleQueue.Enqueue(() =>
				{
					OnAwardPromo(eventData, () =>
					{
						VisibleNext(true);
					});
				});
				VisibleNext();
			}
		}


		/// <summary>
		/// Отоюражение промоушенс
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="OnComplete"></param>
		private void OnAwardPromo(PromotionEventData eventData, Action OnComplete)
		{
			Debug.Log("OnAwardPromo");
			var prefab = PromotionController.Instance.GetPrefabiconeByType(eventData.Category);

			GameObject instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			if (instIcone == null)
			{
				PoolerManager.AddPool(prefab.gameObject, 1, 1);
				instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			}
			_prefabInst = instIcone;
			_prefabName = prefab.gameObject.name;

			RectTransform rtIcone = instIcone.GetComponent<RectTransform>();
			rtIcone.SetParent(_parentIcons);
			rtIcone.anchoredPosition = Vector2.zero;
			rtIcone.localScale = Vector2.one * 0.6f;

			PromotionIcone icone = rtIcone.GetComponent<PromotionIcone>();
			icone.SetTable(_baseController.GameController.SelectTable);
			icone.OnAward(eventData.Category, eventData.Limit, (int)eventData.Value);
			icone.OnCompleteVisible = () =>
			{
				rtIcone.DOScale(Vector2.zero, 0.3f).OnComplete(() =>
				{
					PoolerManager.Return(prefab.gameObject.name, instIcone);
					_prefabInst = null;
					OnComplete?.Invoke();
				}).SetDelay(0.3f);
			};

		}

		#endregion

		#region Bingo

		public void BingoShow()
		{
			Debug.Log("BingoShow");
			_visibleQueue.Enqueue(() =>
			{
				_BingoShow();
			});
			VisibleNext();
		}

		[ContextMenu("Show bingo")]
		public void _BingoShow()
		{
			Debug.Log("_BingoShow");
			GameObject prefab = Garilla.ResourceManager.GetResource<GameObject>("Game/BingoPlayerIcone");

			GameObject instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			if (instIcone == null)
			{
				PoolerManager.AddPool(prefab.gameObject, 1, 1);
				instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			}
			_prefabInst = instIcone;
			_prefabName = prefab.gameObject.name;


			RectTransform rtIcone = instIcone.GetComponent<RectTransform>();
			rtIcone.SetParent(_parentIcons);
			rtIcone.anchoredPosition = Vector2.zero;
			rtIcone.localScale = Vector2.one;
			Animator bingoAnimator = rtIcone.GetComponent<Animator>();

			bingoAnimator.gameObject.SetActive(true);
			bingoAnimator.SetTrigger("visible");
			var cg = bingoAnimator.GetComponent<CanvasGroup>();
			cg.alpha = 1;

			DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, 0.4f).OnComplete(() =>
			{
				instIcone.SetActive(false);
				PoolerManager.Return(prefab.gameObject.name, instIcone);
				_prefabInst = null;
				VisibleNext(true);
			}).SetDelay(4);

		}

		#endregion

		#region BadBeat

		public void OnBedBeatAward(BedBeatAwardData barBeatData){


			if (_baseController == null || _baseController.UserId != barBeatData.UserId) return;
			if (_baseController.GameController == null || _baseController.GameController.SelectTable.id != barBeatData.TableId) return;

			_visibleQueue.Enqueue(() =>
			{
				BadBeatShow();
			});
			VisibleNext();
		}


		[ContextMenu("Bad Beat")]
		public void BadBeatShow()
		{
			GameObject prefab = Garilla.ResourceManager.GetResource<GameObject>("Game/BadBeatPlayericone");

			GameObject instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			if (instIcone == null)
			{
				PoolerManager.AddPool(prefab.gameObject, 1, 1);
				instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			}
			_prefabInst = instIcone;
			_prefabName = prefab.gameObject.name;


			RectTransform rtIcone = instIcone.GetComponent<RectTransform>();
			rtIcone.SetParent(_parentIcons);
			rtIcone.anchoredPosition = Vector2.zero;
			rtIcone.localScale = Vector2.one;
			Animator bingoAnimator = rtIcone.GetComponent<Animator>();

			bingoAnimator.gameObject.SetActive(true);
			var cg = bingoAnimator.GetComponent<CanvasGroup>();
			cg.alpha = 1;

			DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, 0.4f).OnComplete(() =>
			{
				instIcone.SetActive(false);
				PoolerManager.Return(prefab.gameObject.name, instIcone);
				_prefabInst = null;
				VisibleNext(true);
			}).SetDelay(2);

		}

		#endregion

		#region Jackpot


		public void JackpotAward(JackpotAwardData jackpotData)
		{
			if (_baseController == null || _baseController.UserId != jackpotData.UserId) return;
			if (_baseController.GameController == null || _baseController.GameController.SelectTable.id != jackpotData.TableId) return;

			_visibleQueue.Enqueue(() =>
			{
				JackpotShow();
			});
			VisibleNext();
		}

		[ContextMenu("Jackpot")]
		public void JackpotShow()
		{
			GameObject prefab = Garilla.ResourceManager.GetResource<GameObject>("Game/JackpotPlayericone");

			GameObject instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			if (instIcone == null)
			{
				PoolerManager.AddPool(prefab.gameObject, 1, 1);
				instIcone = PoolerManager.Spawn(prefab.gameObject.name);
			}
			_prefabInst = instIcone;
			_prefabName = prefab.gameObject.name;


			RectTransform rtIcone = instIcone.GetComponent<RectTransform>();
			rtIcone.SetParent(_parentIcons);
			rtIcone.anchoredPosition = Vector2.zero;
			rtIcone.localScale = Vector2.one;
			Animator bingoAnimator = rtIcone.GetComponent<Animator>();

			bingoAnimator.gameObject.SetActive(true);
			var cg = bingoAnimator.GetComponent<CanvasGroup>();
			cg.alpha = 1;

			DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0, 0.4f).OnComplete(() =>
			{
				instIcone.SetActive(false);
				PoolerManager.Return(prefab.gameObject.name, instIcone);
				_prefabInst = null;
				VisibleNext(true);
			}).SetDelay(2);

		}

		#endregion

	}
}
