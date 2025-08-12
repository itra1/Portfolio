using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Main.SinglePages;
using it.Animations;

namespace it.UI
{
	public class UserProfilRank : MonoBehaviour
	{
		//public UnityEngine.Events.UnityAction OnExpandClickAction;

		[SerializeField] private TMValueIntAnimate _spsLabel;
		[SerializeField] private it.Animations.ScaleX _progressValue;
		[SerializeField] private RectTransform _currentBlock;
		[SerializeField] private TMValueIntAnimate _currentRankLabel;
		[SerializeField] private RectTransform _nextBlock;
		[SerializeField] private TMValueIntAnimate _nextRankLabel;
		[SerializeField] private RankCard _currentImage;
		[SerializeField] private RankCard _nextImage;

		private void OnEnable()
		{
			//_spsLabel.text = $"{UserController.Instance.Rank.Points} / {1} SPS";
			FillData();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserRankUpdate, UserRankUpdate);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserRankUpdate, UserRankUpdate);
		}

		private void UserRankUpdate(com.ootii.Messages.IMessage handler)
		{
			if (UserController.User == null) return;

			FillData();
		}

		private void FillData()
		{
			int spriteId = 1;
#if !UNITY_STANDALONE
			spriteId = 2;
			#endif

			if (_spsLabel != null)
			{
				_spsLabel.Patern = "0 / 0 SPs";
#if !UNITY_STANDALONE
				_spsLabel.Patern = "0 / 0";
#endif
				_spsLabel.StartValue = 0;
				_spsLabel.EndValue = 0;
			}

			if (_currentRankLabel != null)
				_currentRankLabel.EndValue = 0;
			if (_nextRankLabel != null)
				_nextRankLabel.EndValue = 0;
			if (UserController.Instance.Rank == null || UserController.Instance.Rank.current_rank == null)
			{
				if (_currentBlock != null)
					_currentBlock.gameObject.SetActive(false);
				_currentImage.gameObject.SetActive(false);
				_currentRankLabel.EndValue = 0;
			}
			if (UserController.Instance.Rank == null || UserController.Instance.Rank.next_rank == null)
			{
				if (_nextBlock != null)
					_nextBlock.gameObject.SetActive(false);
				if (_nextImage != null)
					_nextImage.SetData(spriteId, UserController.Instance.Rank.next_rank);
				if (_nextRankLabel != null)
					_nextRankLabel.EndValue = 0;
				if (_currentBlock != null)
					_currentBlock.anchoredPosition = new Vector2(0, _currentBlock.anchoredPosition.y);
				_spsLabel.Patern = "{0} SPs";
#if !UNITY_STANDALONE
				_spsLabel.Patern = "{0}";
#endif
				_spsLabel.EndValue = (int)UserController.Instance.Rank.current_points;
				_progressValue.StartValue = 0;
				_progressValue.EndValue = 1;
			}
			else
			{
				if (_currentBlock != null)
					_currentBlock.anchoredPosition = new Vector2(-33.4f, _currentBlock.anchoredPosition.y);
			}
			if (UserController.Instance.Rank != null && UserController.Instance.Rank.current_rank != null)
			{
				var cur = it.Settings.UserSettings.Ranks.Find(x => x.Id == UserController.Instance.Rank.current_rank.id);
				if (_currentBlock != null)
					_currentBlock.gameObject.SetActive(true);
				_currentImage.gameObject.SetActive(true);
				_currentImage.SetData(spriteId, UserController.Instance.Rank.current_rank);

				_currentRankLabel.EndValue = (int)UserController.Instance.Rank.current_rank.rakeback;

				if (UserController.Instance.Rank.next_rank != null)
				{
					_spsLabel.Patern = "{0} / " + it.Helpers.Currency.String(UserController.Instance.Rank.next_rank.price, false) + " SPs";
#if !UNITY_STANDALONE
					_spsLabel.Patern = "{0} / " + it.Helpers.Currency.String(UserController.Instance.Rank.next_rank.price, false);
#endif
					_spsLabel.EndValue = (int)UserController.Instance.Rank.current_points;
					//_spsLabel.text = $"{UserController.Instance.Rank.CurrentRank.Price} / {UserController.Instance.Rank.NextRank.Price} SPs";
					_progressValue.StartValue = 0;

					_progressValue.EndValue = (float)(UserController.Instance.Rank.current_points / UserController.Instance.Rank.next_rank.price);
				}
				else
				{
					_spsLabel.Patern = "{0} SPs";
#if !UNITY_STANDALONE
					_spsLabel.Patern = "{0}";
#endif
					_spsLabel.EndValue = (int)UserController.Instance.Rank.current_points;
					_progressValue.StartValue = 0;
					_progressValue.EndValue = 1;
				}
				//_progressValue.GetComponent<RectTransform>().localScale = new Vector3((float)(UserController.Instance.Rank.CurrentRank.Price / UserController.Instance.Rank.NextRank.Price), 1, 1);
			}
			if (UserController.Instance.Rank != null && UserController.Instance.Rank.next_rank != null)
			{
				var next = it.Settings.UserSettings.Ranks.Find(x => x.Id == UserController.Instance.Rank.next_rank.id);
				if (_nextBlock != null)
					_nextBlock.gameObject.SetActive(true);
				if (_nextImage != null)
				{
					_nextImage.gameObject.SetActive(true);
					_nextImage.SetData(spriteId, UserController.Instance.Rank.next_rank);
				}

				if (_nextRankLabel != null)
					_nextRankLabel.EndValue = (int)UserController.Instance.Rank.next_rank.rakeback;
				//_spsLabel.Patern = "{0} / " + it.Helpers.Currency.String(UserController.Instance.Rank.NextRank.Price, false)   + " SPs";
				//_spsLabel.EndValue = (int)UserController.Instance.Rank.CurrentRank.Price;
				//_progressValue.StartValue = 0;
				//_progressValue.EndValue = (float)(UserController.Instance.Rank.CurrentRank.Price / UserController.Instance.Rank.NextRank.Price);
			}

			it.Logger.Log(_progressValue.EndValue);
		}

		// Расширение
		public void ExpandButtonTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.Rank);
			//OnExpandClickAction?.Invoke();
		}

	}
}