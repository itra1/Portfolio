using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Configs
{

  public static bool IsSpirit;
  public static bool IsGood;

  [QFSW.QC.Command("god","Режим бессмертия")]
  public static void SetGood()
  {
	 IsGood = !IsGood;

	 Debug.Log("Режим бога " + (IsGood ? "включен" : "выключен"));

  }
  [QFSW.QC.Command("spirit","Режим полета")]
  public static void SetSpirit()
  {
	 IsSpirit = !IsSpirit;

	 Debug.Log("Режим духа " + (IsSpirit ? "включен" : "выключен"));

  }


}
