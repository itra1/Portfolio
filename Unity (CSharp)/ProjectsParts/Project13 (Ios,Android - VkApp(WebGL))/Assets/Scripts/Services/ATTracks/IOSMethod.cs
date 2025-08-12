#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
#endif
using UnityEngine;

public class IOSMethod : MonoBehaviour {
#if UNITY_IOS
    public System.Action<int> OnComplete;

    public void GetAuthorizationStatus()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int curStatus = ATTAuth.GetAppTrackingAuthorizationStatus();
            Debug.Log($"Current status {curStatus}");
            if (curStatus == 0)
            {
                ATTAuth.RequestTrackingAuthorizationWithCompletionHandler((status) =>
                {
                    Debug.Log("ATT status :" + status);
                    curStatus = status;
                });
            }
            OnComplete?.Invoke(curStatus);
        }
    }
#endif
}
