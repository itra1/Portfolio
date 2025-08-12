using UnityEngine;
using System.Collections;

public delegate void Actione();
public delegate void Actione<in T1>(T1 param1);
public delegate void Actione<in T1, in T2>(T1 param1, T2 param2);
public delegate void Actione<in T1, in T2, in T3, in T4, in T5, in T6>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

/// <summary>
/// Стуктура времени
/// </summary>
public struct TimeStruct {
  public int day;
  public int hour;
  public int minut;
  public int second;
  public ulong unixTimeDalta;
  public ulong unixTimeMillisecondsDalta;
  public string timeMinutSecond { get { return string.Format("{0}:{1:d2}", minut, second); } }
}

public class Util : MonoBehaviour {
  
  public static ulong unixTime {
    get { return (ulong)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds; }
  }
  public static ulong unixTimeUniversal {
    get { return (ulong)(System.DateTime.Now.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds; }
  }
  public static int unixDeltaTimeZone {
    get { return (int)(System.DateTime.Now - System.DateTime.Now.ToUniversalTime()).TotalSeconds; }
  }
  public static ulong unixTimeMilliseconds {
    get {
      return (ulong)System.DateTime.Now.Millisecond + unixTime * 1000; }
  }
  public static ulong unixTimeUnvMilliseconds {
    get {
      return (ulong)System.DateTime.Now.Millisecond + unixTimeUniversal * 1000;
    }
  }

  static TimeStruct timeDecrementData = new TimeStruct();

  public static TimeStruct GetTimeDecrement(int unixTimeWait, bool isUniversal = false, bool isMilliseconds = false) {

    if(isMilliseconds) unixTimeWait /= 1000;

    timeDecrementData.unixTimeDalta = (ulong)unixTimeWait - (isUniversal ? unixTimeUniversal : unixTime) ;
    timeDecrementData.day = (int)timeDecrementData.unixTimeDalta / (60 * 60 * 24);
    timeDecrementData.hour = (int)(timeDecrementData.unixTimeDalta - (ulong)(timeDecrementData.day * 60 * 60 * 24)) / (60 * 60);
    timeDecrementData.minut = (int)(timeDecrementData.unixTimeDalta - (ulong)(timeDecrementData.day * 60 * 60 * 24) - (ulong)(timeDecrementData.hour * 60 * 60)) / 60;
    timeDecrementData.second = (int)timeDecrementData.unixTimeDalta - (timeDecrementData.day * 60 * 60 * 24) - (timeDecrementData.hour * 60 * 60) - (timeDecrementData.minut * 60);

    return timeDecrementData;
  }
}

public static class Helpers {

  public static Coroutine Invoke(this MonoBehaviour monoBehaviour, Actione action, float time) {
    return monoBehaviour.StartCoroutine(InvokeImpl(action, time));
  }

  private static IEnumerator InvokeImpl(Actione action, float time) {
    yield return new WaitForSeconds(time);

    action();
  }
}