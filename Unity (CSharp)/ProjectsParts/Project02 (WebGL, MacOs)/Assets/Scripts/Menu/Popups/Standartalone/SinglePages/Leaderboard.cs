
using UnityEngine;
using it.Api;



namespace it.Main.SinglePages
{
	public class Leaderboard : SinglePage
	{
		[SerializeField] private Page[] _pages;
		[SerializeField] private it.Inputs.CurrencyLabel[] _winAmountLabels;

		[SerializeField] private LeaderBoardInfo leaderBoardInfo;
		[SerializeField] private MainLeader mainLeader;

		//private const string LEADERBOARD = "/leader_board";
		//private const string WINNER = "/leader_board/winner_prev_week";

		private int _index = 0;


		//private readonly TypeBoard[] _typeBoards =
		//{
		//				new TypeBoard("micro"),
		//				new TypeBoard("average"),
		//				new TypeBoard("high")
		//		};
		private readonly string[] _typeBoards = new string[]	{"micro","average","high"	};

		[System.Serializable]
		private struct Page
		{
			public it.UI.Elements.FilterSwitchButtonUI Button;
			public double[] Values;
		}


		protected override void EnableInit()
		{
			base.EnableInit();

			for (int i = 0; i < _pages.Length; i++)
			{
				int index = i;
				string state;
				var p = _pages[i];
				p.Button.OnClickPointer.RemoveAllListeners();
				p.Button.OnClickPointer.AddListener(() => { VisiblePage(index); });
			}

			VisiblePage(0);
		}

		public void VisiblePage(int index)
		{
			_index = index;
			ConfirmData();
			LoadLeaderBoardInfo(_typeBoards[_index]);
		}

		private void ConfirmData()
		{
			var struc = _pages[_index];

			for (int i = 0; i < _pages.Length; i++)
			{
				_pages[i].Button.IsSelect = _index == i;
			}

			for (int i = 0; i < _winAmountLabels.Length; i++)
			{
				_winAmountLabels[i].SetValue(struc.Values[i]);
			}
		}

		private void LoadLeaderBoardInfo(string type)
		{
		if(leaderBoardInfo != null)
			AppApi.GetLeaderboard(type, (result) =>
			{
				if (result.IsSuccess)
					leaderBoardInfo.SetData(result.Result);
			});
			AppApi.GetLeaderboardLastWinner(type, (result) =>
			{
				//if (result.IsSuccess)
					mainLeader.SetMainPlayer(result.Result);
			});


			//AppApi.LeaderBoardInfo(LEADERBOARD, type, result =>
			//		{
			//			if (result != null)
			//			{
			//				leaderBoardInfo.SetData(result);
			//			}
			//		},
			//		(error) => { leaderBoardInfo.SetData(null); });

			//AppApi.LeaderBoardInfo(WINNER, type, result =>
			//{
			//	if (result != null)
			//	{
			//		mainLeader.SetMainPlayer(result);
			//	}
			//},
			//		(error) => { mainLeader.SetDefault(); });

		}
	}
}
