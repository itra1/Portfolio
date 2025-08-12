#if UNITY_IOS

using System;
using System.Collections;
using Unity.Advertisement.IosSupport;
using UnityEngine;
using UnityEngine.iOS;

namespace Platforms.IOS
{
  public class AttHelper
  {
    public const string LogKey = "[ATT Request]";
    public static IEnumerator AttRequest()
    {
      Debug.Log($"{LogKey} Init request");
      var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
      Version currentVersion = new(Device.systemVersion);
      Version ios14 = new("14.5");
      bool isRequestComplete = false;

      Debug.Log($"{LogKey} current status {status.ToString()}");
      Debug.Log($"{LogKey} current version {currentVersion}");

      if (status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED || currentVersion < ios14)
      {
        Debug.Log($"{LogKey} breake");
        yield break;
      }

        Debug.Log($"{LogKey} request");
      ATTrackingStatusBinding.RequestAuthorizationTracking(status =>
      {
        isRequestComplete = true;
        Debug.Log($"{LogKey} complete request");
      });

      while (!isRequestComplete)
        yield return null;
        Debug.Log($"{LogKey} finish");
    }
  }
}
#endif