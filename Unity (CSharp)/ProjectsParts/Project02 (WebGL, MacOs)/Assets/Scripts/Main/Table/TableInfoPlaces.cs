using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Main;
using it.Network.Rest;
using Garilla.Games;

namespace it.UI
{
	public class TableInfoPlaces : MonoBehaviour
	{
		public bool IsBig;
		public List<PlayerGameIcone> PlayerIcons;

		public void SetData(Table record)
		{
			PlayerIcons.ForEach(x =>
			{
				if (x.DataRect != null)
					x.DataRect.gameObject.SetActive(false);
				if (x.EmptyRect != null)
					x.EmptyRect.gameObject.SetActive(true);
			});
			for (int i = 0; i < record.table_player_sessions.Length; i++)
			{
				PlayerGameIcone player = PlayerIcons[record.table_player_sessions[i].place - 1];
				if (player.DataRect != null)
					player.DataRect.gameObject.SetActive(true);
				if (player.EmptyRect != null)
					player.EmptyRect.gameObject.SetActive(false);
				player.gameObject.SetActive(true);
				player.Set(record.table_player_sessions[i]);
			}
			ReservedPlaces(record.table_player_reservations);
		}

		public void ReservedPlaces(List<PlaceReserve> reserves)
		{
			for (int i = 0; i < PlayerIcons.Count; i++)
			{
				var place = reserves.Find(x => x.place - 1 == i && x.FinishDate > GameHelper.NowTime);
				if (place != null)
				{
					PlayerIcons[i].EmptyRect.SetReserved(true);
				}
				else
					PlayerIcons[i].EmptyRect.SetReserved(false);
			}

		}

	}
}