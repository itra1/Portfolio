using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using it.Network.Rest;
using Sett = it.Settings;
using DG.Tweening;

namespace it.UI
{
	public class TableInfoPanel : MonoBehaviour
	{
		private List<TableInfoItem> _tableInfoList = new List<TableInfoItem>();
		//private TableInfoItem _tableInfo;
		private it.Network.Rest.Table _selectTable;

		private void Awake()
		{
			//gameObject.SetActive(false);
		}

		private void OnEnable()
		{
		}


		public void VisiblePanel(it.Network.Rest.Table table)
		{
			SpawnPanel(table);
			gameObject.SetActive(true);
			RectTransform rt = GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(rt.rect.width + 100, rt.anchoredPosition.y);
			rt.DOAnchorPos(Vector2.zero, 0.3f);
		}

		private void SpawnPanel(it.Network.Rest.Table table)
		{

			for (int i = 0; i < _tableInfoList.Count; i++)
			{
				_tableInfoList[i].gameObject.SetActive(false);
				if (_tableInfoList[i].GameTypes.Contains((GameType)table.game_rule_id)
				&& _tableInfoList[i].IsAllOrNofing == table.is_all_or_nothing 
				&& _tableInfoList[i].IsDealerChoise == table.is_dealer_choice)
				{
					_tableInfoList[i].gameObject.SetActive(true);
					return;
				}
			}

			var list = it.Settings.GameSettings.GameInfoPanelBlock.InfoList;

			for(int i = 0; i < list.Length;i++){

				if (list[i].GameTypes.Contains((GameType)table.game_rule_id) && list[i].IsAllOrNofing == table.is_all_or_nothing && list[i].IsDealerChoise == table.is_dealer_choice)
				{
					GameObject inst = Instantiate(list[i].gameObject, transform);
					inst.transform.localPosition = Vector3.zero;
					inst.transform.localScale = Vector3.one;
					inst.transform.localRotation = Quaternion.identity;
					RectTransform rt = inst.GetComponent<RectTransform>();
					rt.anchoredPosition = Vector2.zero;
					rt.sizeDelta = Vector2.zero;
					TableInfoItem com = inst.GetComponent<TableInfoItem>();
					_tableInfoList.Add(com);
					inst.gameObject.SetActive(true);
				}

			}

		}

		public void HidePahel()
		{
			RectTransform rt = GetComponent<RectTransform>();
			rt.DOAnchorPos(new Vector2(rt.rect.width + 100, rt.anchoredPosition.y), 0.3f).OnComplete(() =>
			{
				gameObject.SetActive(false);
			});
		}

		public void BackButtonTouch()
		{
			HidePahel();
		}
	}
}