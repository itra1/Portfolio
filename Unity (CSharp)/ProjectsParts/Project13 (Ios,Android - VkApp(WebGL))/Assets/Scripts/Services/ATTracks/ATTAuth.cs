#if UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
#endif

public class ATTAuth {
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _RequestTrackingAuthorizationWithCompletionHandler();

    [DllImport("__Internal")]
    private static extern int _GetAppTrackingAuthorizationStatus();

    private static Action<int> getAuthorizationStatusAction;

    /// <summary>
    ///Request ATT authorization window
    /// </summary>
    /// <param name="getResult"></param>
    public static void RequestTrackingAuthorizationWithCompletionHandler(Action<int> getResult)
    {
        //-1:"ios version < 14"
        //0: "ATT authorization status to be determined ";
        //1: "ATT authorization status is limited ";
        //2: "ATT has rejected ";
        //3: "Authorized by ATT ";
        Debug.Log("RequestTrackingAuthorizationWithCompletionHandler");
        getAuthorizationStatusAction = getResult;
        _RequestTrackingAuthorizationWithCompletionHandler();
    }

    /// <summary>
    ///Obtain the ATT authorization status
    /// </summary>
    /// <returns></returns>
    public static int GetAppTrackingAuthorizationStatus()
    {
        return _GetAppTrackingAuthorizationStatus();
    }

    public void GetAuthorizationStatus(string status)
    {
        getAuthorizationStatusAction?.Invoke(int.Parse(status));
    }
#endif
}
