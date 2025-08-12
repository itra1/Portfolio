using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

  public static string RoundValueString(long value) {

    if (value / 1000000000000000000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000000000000000000), 1)).ToString() + "КВИ";
    if (value / 1000000000000000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000000000000000), 1)).ToString() + "КВ";
    if (value / 1000000000000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000000000000), 1)).ToString() + "TР";
    if (value / 1000000000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000000000), 1)).ToString() + "МЛ";
    if (value / 1000000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000000), 1)).ToString() + "М";
    if (value / 1000.0 > 0.5)
     return (System.Math.Round(((double)value / 1000), 1)).ToString() + "K";

    return (System.Math.Round((double)value, 1)).ToString();

  }
  public static string RoundValueString(float value) {
    return RoundValueString((long)value);
  }
  public static string RoundValueString(int value) {
    return RoundValueString((long)value);
  }


}
