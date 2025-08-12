using UnityEngine;

namespace it.Main
{
	public class RankUserPanel : MonoBehaviour
	{
		[SerializeField] private it.Main.SinglePages.RankCard _card;
		[SerializeField] private it.Inputs.CurrencyLabel _spsLabel;
		[SerializeField] private RectTransform _progress;

		private void Awake()
		{
			FillData();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserRankUpdate, UserRankUpdate);
		}

		private void UserRankUpdate(com.ootii.Messages.IMessage handler)
		{
			FillData();
		}

		private void FillData()
		{
			var rk = UserController.Instance.Rank;
			if (rk == null)
			{
				_spsLabel.SetValue(0);
				_progress.localScale = new Vector3(0, 1, 1);
				return;
			}

			_card.SetData(2, rk.current_rank);
			_spsLabel.SetValue(rk.current_points);
			if (rk.next_rank == null)
				_progress.localScale = new Vector3(1, 1, 1);
			else
				_progress.localScale = new Vector3((float)(rk.current_points / rk.next_rank.price), 1, 1);
		}

		public void ClickPanelTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.Rank);
		}

	}
}