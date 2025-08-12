using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.UI;
using System.Linq;
using it.Network.Rest;
using it.Popups;
using it.UI.Elements;
using it.Settings;

namespace Garilla.UI
{
	public class TableInfoManager : MonoBehaviour
	{
		//private List<TableInfoItem> _pagesList = new List<TableInfoItem>();

		private void Awake()
		{
			//_pagesList = gameObject.GetComponentsInChildren<TableInfoItem>(true).ToList();
			//_pagesList.ForEach(x => x.gameObject.SetActive(false));
		}

		private void OnDisable()
		{
			DisableAll();
		}

		public void DisableAll()
		{
			//_pagesList.ForEach(x => x.gameObject.SetActive(false));
		}

		/// <summary>
		/// Открытие
		/// </summary>
		/// <param name="table"></param>
		public void Open(Table table)
		{
			var gameSettings
#if UNITY_STANDALONE
			= it.Settings.StandaloneSettings.GameInfoPrefabs
#else
			= it.Settings.AndroidSettings.GameInfoPrefabs
#endif
			.Find(x =>
			x.GameTypes.Contains((GameType)table.game_rule_id)
			&& x.IsAllOrNofing == table.is_all_or_nothing
			&& x.IsDealerChoise == table.is_dealer_choice);

			if (gameSettings == null) return;

			//var go = Resources.Load<GameObject>("Prefabs/UI/Lobby/Info/" + gameSettings.PrefabName);
			var go = (GameObject)Garilla.ResourceManager.GetResource<GameObject>("Prefabs/UI/Lobby/Info/" + gameSettings.PrefabName);

			var inst = Instantiate(go, transform);

			var rt = inst.GetComponent<RectTransform>();
			rt.anchoredPosition = Vector2.zero;
			rt.localScale = Vector2.one;
			rt.sizeDelta = Vector2.zero;

			inst.gameObject.SetActive(true);

			var bb = inst.GetComponentInChildren<BackButton>();

			if (bb == null)
			{
				it.Logger.Log("[INFO TABLE] No exists back button");
				return;
			}

			var gb = bb.GetComponent<GraphicButtonUI>();

			gb.OnClick.RemoveAllListeners();
			gb.OnClick.AddListener(() =>
			{
				inst.gameObject.SetActive(false);
				Destroy(inst, 1);
			});

		}

	}
}