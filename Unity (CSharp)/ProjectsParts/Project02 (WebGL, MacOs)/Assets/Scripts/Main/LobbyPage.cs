using it.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;
using it.Settings;

namespace it.Main
{
  public class LobbyPage : MainContentPage
  {
    private List<SheetBase> _pages;

    private TablePanel _table;
    public GameObject TimeBankPanel;
    public GameObject ExtraTimePanel;
    public GameObject AntePanel;

    private void Awake()
    {
      _table = GetComponentInChildren<TablePanel>(true);
      _pages = GetComponentsInChildren<SheetBase>(true).ToList();
    }
		private void OnEnable()
    {
      TimeBankPanel.SetActive(false);
      ExtraTimePanel.SetActive(false);
      AntePanel.SetActive(false);
    }

		public void SelectPage(LobbyType dialog)
    {
      for (int i = 0; i < _pages.Count; i++)
      {
        _pages[i].gameObject.SetActive(_pages[i].Lobby == dialog);
      }
      //SelectSheetRecord(dialog, null);
    }

  }
}