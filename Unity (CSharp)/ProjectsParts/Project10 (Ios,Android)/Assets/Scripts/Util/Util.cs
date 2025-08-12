using UnityEngine;
using System.Collections;

/// <summary>
/// Стуктура времени
/// </summary>
public struct TimeStruct {
  public int day;
  public int hour;
  public int minut;
  public int second;
  public int unixTimeDalta;
}

public delegate void EventAction();
public delegate void EventAction<in T>(T arg1);
public delegate void EventAction<in T1 ,in T2>(T1 arg1, T2 arg2);

public class Util : MonoBehaviour
{
  public static int unixTime {
    get { return (int)( System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0) ).TotalSeconds; }
  }

  static TimeStruct timeDecrementData = new TimeStruct();

  public static TimeStruct GetTimeDecrement(int unixTimeWait) {

    timeDecrementData.unixTimeDalta = unixTimeWait - unixTime;
    timeDecrementData.day = timeDecrementData.unixTimeDalta / ( 60 * 60 * 24 );
    timeDecrementData.hour = ( timeDecrementData.unixTimeDalta - ( timeDecrementData.day * 60 * 60 * 24 ) ) / ( 60 * 60 );
    timeDecrementData.minut = ( timeDecrementData.unixTimeDalta - ( timeDecrementData.day * 60 * 60 * 24 ) - ( timeDecrementData.hour * 60 *60 ) ) / 60;
    timeDecrementData.second =  timeDecrementData.unixTimeDalta - ( timeDecrementData.day * 60 * 60 * 24 ) - ( timeDecrementData.hour * 60 * 60 ) - (timeDecrementData.minut * 60);

    return timeDecrementData;
  }

}

public static class Helpers {


  public static Coroutine Invoke(this MonoBehaviour monoBehaviour, EventAction action, float time) {
    return monoBehaviour.StartCoroutine(InvokeImpl(action, time));
  }

  private static IEnumerator InvokeImpl(EventAction action, float time) {
    yield return new WaitForSeconds(time);

    action();
  }
}