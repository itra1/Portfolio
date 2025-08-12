using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace it.Mobile.Lobby
{
	public class LobbyNavigations : MonoBehaviour
	{
		public UnityAction<LobbyType> OnSelectPage;


		public void SetHoldemList()
		{
			OnSelectPage?.Invoke(LobbyType.Holdem);
		}
		public void SetPloList()
		{
			OnSelectPage?.Invoke(LobbyType.Plo);
		}
		public void SetShortDeskList()
		{
			OnSelectPage?.Invoke(LobbyType.ShortDesk);
		}
		public void SetAllOrNothingList()
		{
			OnSelectPage?.Invoke(LobbyType.AllOrNothing);
		}
		public void SetOfcList()
		{
			OnSelectPage?.Invoke(LobbyType.Ofc);
		}
		public void SetMemphisList()
		{
			OnSelectPage?.Invoke(LobbyType.Mempfis);
		}
		public void SetFaceToFaceList()
		{
			OnSelectPage?.Invoke(LobbyType.FaceToFace);
		}
		public void SetOklahomaList()
		{
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}
		public void SetVipGameList()
		{
			OnSelectPage?.Invoke(LobbyType.VipGame);
		}
		public void SetMttList()
		{
			OnSelectPage?.Invoke(LobbyType.Mtt);
		}
		public void SetMontanaList()
		{
			//OnSelectPage?.Invoke(LobbyType.Montana);
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}
		public void SetDealerChoiceList()
		{
			OnSelectPage?.Invoke(LobbyType.DealerChoice);
		}

	}
}