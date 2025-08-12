using UnityEngine;
using UnityEngine.UI;
using it.UI;

namespace it.Main
{
	public class TableNavigation : MonoBehaviour
	{
		public MainPagesType Page { get; private set; }

		[SerializeField] private PageButtons[] Pages;

		[System.Serializable]
		public struct PageButtons
		{
			public Image Image;
			public MainPagesType Page;
			public LobbyType Lobby;
			[HideInInspector] public it.UI.Elements.GraphicButtonUI Button;
		}

		private Color _standardColor = Color.white;
		private Color _deselectColor = Color.white;

		private void OnEnable()
		{
			_deselectColor.a = 0.4f;
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.PageSelect, PageSelect);
			HomeButton();
			DG.Tweening.DOVirtual.DelayedCall(0.5f, () =>
			{
				HomeButton();
			});
		}

		

		//private void PageSelect(com.ootii.Messages.IMessage handles)
		//{
		//	OppenPage();
		//}

		public void SetPage(MainPagesType page, LobbyType lobby)
		{
			Page = page;

			if(Page == MainPagesType.Lobby){
				_deselectColor.a = 0.4f;
				_standardColor = Color.white;
			}
			else
			{
				_deselectColor.a = 1f;
				_standardColor = Color.white * 1.3f;
			}


			for (int i = 0; i < Pages.Length; i++)
			{
				if (Pages[i].Button == null)
					Pages[i].Button = Pages[i].Image.GetComponent<it.UI.Elements.GraphicButtonUI>();

				if (lobby == Pages[i].Lobby)
				{
					//Pages[i].Image.color = Color.white;
					Pages[i].Button.StartColor = _standardColor;
					Pages[i].Button.NormalState();
				}
				else
				{
					//Pages[i].Image.color = _deselectColor;
					Pages[i].Button.StartColor = _deselectColor;
					Pages[i].Button.NormalState();
				}
			}
		}
		public void SetLobby(LobbyType lobby)
		{
			for (int i = 0; i < Pages.Length; i++)
			{
				if (Pages[i].Button == null)
					Pages[i].Button = Pages[i].Image.GetComponent<it.UI.Elements.GraphicButtonUI>();

				if (lobby == Pages[i].Lobby)
				{
					//Pages[i].Image.color = Color.white;
					Pages[i].Button.StartColor = _standardColor;
					Pages[i].Button.NormalState();
				}
				else
				{
					//Pages[i].Image.color = _deselectColor;
					Pages[i].Button.StartColor = _deselectColor;
					Pages[i].Button.NormalState();
				}
			}
		}

		private void OpenPage(MainPagesType dialog)
		{
			//for (int i = 0; i < Pages.Length; i++)
			//{
			//	Pages[i].Image.color = dialog == Pages[i].Type ? Color.gray : Color.white;
			//}
			//it.UI.MainPanel mm = GetComponentInParent<it.UI.MainPanel>();
			//var comp = mm.GetComponentInChildren<MainBody>();
			//comp.SetPage(dialog);
			//if (dialog == DialogType.Main_Lobby)
			//{
			//	var comp1 = mm.GetComponentInChildren<LobbyPage>();
			//	comp1.SelectPage(dialog);
			//}
		}

		public void HomeButton()
		{
			GetComponentInParent<MainPanel>().HomeButton();

			//OpenPage(MainPagesType.Home);
		}

		public void NavHoldemButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.Holdem);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_Holdem);
		}
		public void NavPLOButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.Plo);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_Plo);
		}
		public void NavAllOrNothingButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.AllOrNothing);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_AllOrNothing);
		}
		public void NavShortDeckButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.ShortDesk);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_ShortDesk);
		}
		public void NavOFCButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.Ofc);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_Ofc);
		}
		public void NavMemphisButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.Mempfis);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_Ofc);
		}
		public void NavFaceToFaceButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.FaceToFace);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_FaceToFace);
		}

		public void NavOklahomaButton()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}

		public void NavVipGameButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.VipGame);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_VipGame);
		}
		public void NavDealerChoiceButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.DealerChoice);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_VipGame);
		}
		public void NavMTTButton()
		{
			GetComponentInParent<MainPanel>().LobbyButton(LobbyType.Mtt);
			//OpenPage(MainPagesType.Lobby);
			//OppenPage(DialogType.Main_Lobby_Mtt);
		}

		public void DevelopButtonTouch(){
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}

	}
}