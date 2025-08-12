using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using it.Network.Rest;
using it.Popups;

namespace it.Popups
{
  public class UITablenInfo : PopupBase
  {
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Blinds;
    [SerializeField] private TextMeshProUGUI Ante;
    [SerializeField] private TextMeshProUGUI Currency;
    [SerializeField] private TextMeshProUGUI BuyIn;
    [SerializeField] private TextMeshProUGUI MinPlayers;
    [SerializeField] private TextMeshProUGUI MaxPlayers;
    [SerializeField] private TextMeshProUGUI TimeToAct;
    [SerializeField] private TextMeshProUGUI TimeBank;
    [SerializeField] private TextMeshProUGUI DisconnectTime;
    [SerializeField] private TextMeshProUGUI SitOutTime;
    [SerializeField] private TextMeshProUGUI AllIn;
    [SerializeField] private TextMeshProUGUI Straddle;

    public void Init(Table table)
    {
      gameObject.SetActive(true);

      Title.text = table.name;
      Blinds.text = table.big_blind_size.ToString();
      Ante.text = "Not_Found"; //TO DO
      Currency.text = "Not_Found"; //TO DO
      BuyIn.text = $"${table.BuyInMinEURO} - ${table.BuyInMaxEURO}\n";
      MinPlayers.text = $"{table.players_autostart_count} Players";
      MaxPlayers.text = $"{table.MaxPlayers} Players";
      TimeToAct.text = $"{table.action_time}";
      TimeBank.text = "Not_Found"; //TO DO
      DisconnectTime.text = "Not_Found"; //TO DO
      SitOutTime.text = $"{table.rest_timeout.Value}";
      AllIn.text = "Not_Found"; //TO DO
      Straddle.text = "Not_Found"; //TO DO
    }

    public void Hide()
    {
      gameObject.SetActive(false);
    }

  }
}