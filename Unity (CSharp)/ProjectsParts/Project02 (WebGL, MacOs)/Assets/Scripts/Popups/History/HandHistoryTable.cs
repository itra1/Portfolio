using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Garilla.Games.UI
{
	public class HandHistoryTable : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<string> Select;

		public LobbyType Table;
		public List<GameType> GameTypes;
		[SerializeField] public Transform[] StagesContent;
		[SerializeField] public string[] StagesName;
		private string stageFinal;
		[SerializeField] private HistoryItemPlayer ItemPlayerLeft;
		[SerializeField] private HistoryItemPlayer ItemPlayerRight;

		public void SelectColum(string name)
		{
			GetComponentInParent<HandHistoryPopup>().StageSelect(name);
			//Select?.Invoke(name);
		}


	}
}