using it.Network.Rest;
using System;
using TMPro;
using UnityEngine;

public class PayoutItem : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Title;
	[SerializeField] private TextMeshProUGUI Game;
	[SerializeField] private it.Inputs.CurrencyLabel Value;

	public void UseData(JackpoBlinds record, decimal valueJackpot)
	{
		Title.text = $" {it.Helpers.Currency.String(record.small_blind)} / {it.Helpers.Currency.String(record.big_blind)}";
		Value.SetValue((decimal)record.payout * valueJackpot);
		Game.text = record.game == "holdem" ? $"<color=#CAA165>{record.game.ToUpper()}</color>" : $"<color=#4A992F>{record.game.ToUpper()}</color>";
	}
}