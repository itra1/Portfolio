using it.Network.Rest;
using System;
using System.Runtime.Serialization;
using UnityEngine;
[Serializable]
public class Blind{
  [Header("Blind Type")]
  public BlindType type;
  [Header("For Multi Type Fields Value")]
  public decimal minValue;
  public decimal maxValue;
  [Header("For Single Type Fields Value")]
  public decimal SingleValue;
  [Header("Buy-In")]
  public decimal minBuyIn;
  public decimal maxBuyIn;
  [Header("Stake Type for Blind")]
  public BlindStakes Stake;
  public string LevelName;
  public float Rake;
  public float RakeCap;
  public string level_id;

	public override string ToString()
  {
    return $"blinds: {minValue}/{maxValue} BuyIn {minBuyIn}/{maxBuyIn} ";
  }
  public string Blinds(){
   var res = "none";
   switch (type){
      case BlindType.Multi:
        res = $"{it.Helpers.Currency.String(minValue)} / {it.Helpers.Currency.String(maxValue)}";
        break;
      case BlindType.Single:
        res = it.Helpers.Currency.String(SingleValue);
        break;
      default:
        res = "none";
        break;
    }

    return res;
  }
  public string MinMaxBuyIin(){
		return $"{it.Helpers.Currency.String(minBuyIn)} / {it.Helpers.Currency.String(maxBuyIn)}";
	}
	public string AnteBuyIin()
  {
    return string.Format("{0} (A{1})", (float)minValue, (float)minValue);
	}

	internal string StakeName()
  {
    if (LevelName != null) return LevelName;
    else return Stake.ToString();
  }
  public enum BlindType{
  Multi,
  Single
  }
}
