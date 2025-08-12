using System.Collections;
using Zenject;
using UnityEngine;
#if FIREBASE_SERVICE
using Firebase.Extensions;
#endif

namespace Core.Engine.Services.FireBService.RemoteConfig
{
#if FIREBASE_SERVICE
	public class FirebaseRemoteConfigInit : IFirebaseRemoteConfigInit
	{
		private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
		private bool isFirebaseInitialized = false;
		public void Init()
		{
			Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
				dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available)
				{
					InitializeFirebase();
				}
				else
				{
					Debug.LogError(
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		}

		void InitializeFirebase()
		{
			// [START set_defaults]
			System.Collections.Generic.Dictionary<string, object> defaults =
				new System.Collections.Generic.Dictionary<string, object>();

			// These are the values that are used if we haven't fetched data from the
			// server
			// yet, or if we ask for values that the server doesn't have:
			defaults.Add("url", "custom");
			defaults.Add("webview", false);
			defaults.Add("organic", false);

			Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
				.ContinueWithOnMainThread(task => {
					// [END set_defaults]
					Debug.Log("RemoteConfig configured and ready!");
					Debug.Log(defaults["url"]);
					Debug.Log(defaults["webview"]);
					Debug.Log(defaults["organic"]);
					Debug.Log(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
							 .GetValue("url").StringValue);
					;
					isFirebaseInitialized = true;
				});

		}

	}
#endif
}