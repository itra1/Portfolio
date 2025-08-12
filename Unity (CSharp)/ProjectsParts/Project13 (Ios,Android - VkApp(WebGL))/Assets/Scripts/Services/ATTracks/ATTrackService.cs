#if UNITY_IOS || UNITY_ANDROID
using System;
using Cysharp.Threading.Tasks;
using Unity.Advertisement.IosSupport;
using UnityEngine;
using UnityEngine.iOS;
#endif

namespace Game.Services.ATTracks {
	public class ATTrackService {
#if UNITY_IOS || UNITY_ANDROID

		public static async UniTask<int> RequestATT() {
            var isWait = false;
            int result = 0;

            IOSMethod met = GameObject.FindAnyObjectByType<IOSMethod>();
            met.OnComplete = (st) => {
                Debug.Log("ATT status :" + st);
                result = st;
                isWait = false;
            };

            await UniTask.Delay(1000);
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                result = ATTAuth.GetAppTrackingAuthorizationStatus();
                Debug.Log($"Current status {result}");
                if (result == 0)
                {
                    isWait = true;


                    ATTAuth.RequestTrackingAuthorizationWithCompletionHandler((status) =>
                    {
                        Debug.Log("ATT status :" + status);
                        result = status;
                        isWait = false;
                    });
                }
            }
            await UniTask.WaitWhile(() => isWait);
            return result;
        }
#endif
	}
}
