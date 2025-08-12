using it.Network.Rest;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;

using static it.Settings.AppSettings;

namespace Garilla.Update
{
	public class UpdateController : MonoBehaviour
	{
		public const int STEP_TO_EXIT = 3;

		public static UpdateController Instance => _instance;
		public string ListUrl =>
#if UNITY_WEBGL
		_webglList;
#elif UNITY_ANDROID
		_androidList;
#elif UNITY_IOS
		_iosList;
#else
		_pcList;
#endif

		private static UpdateController _instance;

		private string _androidList = "/getAndroidList";
		private string _iosList = "/getIosList";
		private string _webglList = "/getWebGLList";
		private string _pcList = "/getGameList";

		private BuildItem _maxVeraionToUpdate;
		private bool _isWaitUpdate;

		private string _launcherPath = null;
		private System.DateTime _lastTimeCheck;
		private bool _isTimerPopup;

		private void Awake()
		{
			_instance = this;
#if UNITY_STANDALONE
			if (AppConfig.TableExe == null && PlayerPrefs.HasKey(StringConstants.APP_UPDATE))
				PlayerPrefs.DeleteKey(StringConstants.APP_UPDATE);

			string launcherPathBase64 = CommandLineController.GetLauncherPath();

			if (launcherPathBase64 != null)
			{
				_launcherPath = Encoding.UTF8.GetString(Convert.FromBase64String(launcherPathBase64));
			}
#endif

			_lastTimeCheck = System.DateTime.Now;
#if !UNITY_EDITOR
			CheckNewVersions(true);
#endif
		}

		private void Update()
		{
			if (!_isWaitUpdate && (System.DateTime.Now - _lastTimeCheck).TotalMinutes >= 1)
			{
				_lastTimeCheck = System.DateTime.Now;
				CheckNewVersions(false);
			}

			if (_isWaitUpdate)
			{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
				if (it.Mobile.OpenTableManager.Instance.OpenPanels.Count <= 0)
					ExistsCriticleUpdate();
#elif UNITY_STANDALONE
				if (StandaloneController.Instance.TableWindows.Count <= 0)
					ExistsCriticleUpdate();
#endif
			}

		}

		[ContextMenu("TestRequestUpdate")]
		private void TestRequestUpdate()
		{
			CheckNewVersions(false);
		}

		private void CheckNewVersions(bool isStart)
		{
			RequestList((error, data) =>
			{
				if (error) return;
				CheckVersion(data, isStart);
			});
		}

		private void RequestList(UnityEngine.Events.UnityAction<bool, List<BuildItem>> callbackSuccess)
		{

#if UNITY_WEBGL

			var url = Application.absoluteURL.Split(new char[] { '?' })[0];

			if (url.Substring(url.Length - 1) == "/")
				url = url.Substring(0, url.Length - 1);

			url += "/release.txt";
			//it.Logger.Log("[UPDATER] REELASES URL " + url);

			it.Managers.NetworkManager.Request(url, (result) =>
			{
				//it.Logger.Log("[UPDATER] REELASES " + result);

				try
				{
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<BuildItem>(result);
					callbackSuccess?.Invoke(false, new List<BuildItem>() { data });
				}
				catch { }

			}, (error) =>
			{
				callbackSuccess?.Invoke(true, null);
			});

#else
			string url = it.Settings.AppSettings.ReleaseServer + ListUrl;

			List<KeyValuePair<string, string>> listHeaders = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("Authorization", "Bearer app_id_5535fg34fd") };

			it.Managers.NetworkManager.Request(url, listHeaders, (result) =>
			{
				it.Logger.Log("[UPDATER] REELASES " + result);

				try
				{
					var data = Newtonsoft.Json.JsonConvert.DeserializeObject<BuildResponce>(result);
					callbackSuccess?.Invoke(data.error, data.items);
				}
				catch { }

			}, (error) =>
			{
				callbackSuccess?.Invoke(true, null);
			});

#endif

		}

		private void CheckVersion(List<BuildItem> buildList, bool isStart = false)
		{
			string currentVersion = Application.version;

			bool existsCriticle = false;

			//it.Logger.Log($"[UPDATER] appcount count {buildList.Count}");

			for (int i = 0; i < buildList.Count; i++)
			{
				//it.Logger.Log($"[UPDATER] build {buildList[i].app_version} is prod {buildList[i].IsProduction} current app {AppConfig.IsDevApp}");
				if (!buildList[i].IsProduction && !AppConfig.IsDevApp) continue;

				if (BuildItem.Max(currentVersion, buildList[i].app_version))
				{
					if (_maxVeraionToUpdate == null)
						_maxVeraionToUpdate = buildList[i];

					//it.Logger.Log("[UPDATER] exist update");
					if (buildList[i].IsCitical)
						existsCriticle = true;
				}
				if (_maxVeraionToUpdate != null)
				{
					if (BuildItem.Max(_maxVeraionToUpdate.app_version, buildList[i].app_version))
					{
						_maxVeraionToUpdate = buildList[i];
					}
				}
			}

			if (existsCriticle)
			{
				ExistsCriticleUpdate();
			}

		}

		public void ExistsCriticleUpdate()
		{
			_isWaitUpdate = true;
			bool isWaitOpenTables = false;
			AppConfig.IsLockedOpenTable = true;
#if UNITY_ANDROID || UNITY_WEBGL

			foreach (var elem in it.Mobile.OpenTableManager.Instance.OpenPanels.Values)
			{
				//if (elem.HashActiveDistribution() && elem.CurrentGameUIManager.HandsBeforeUpdate == null)
				//{
				//	isWaitOpenTables = true;
				//	elem.SetWaitToUpdate(STEP_TO_EXIT);
				//}
				//else
				//	elem.CloseRequest();

				if (!elem.HasInGame())
				{
					elem.CloseRequest();
				}
				else
				{
					isWaitOpenTables = true;
					if (elem.CurrentGameUIManager.HandsBeforeUpdate == null)
						elem.SetWaitToUpdate(STEP_TO_EXIT);
				}


			}
#elif UNITY_STANDALONE
			isWaitOpenTables = StandaloneController.Instance.TableWindows.Count > 0;
#endif

			if (!isWaitOpenTables)
			{
#if UNITY_ANDROID
				it.Main.PopupController.Instance.ShowPopup(PopupType.Update);
#else

				if (!_isTimerPopup)
				{
					_isTimerPopup = true;

					var popup = it.Main.PopupController.Instance.ShowPopup<it.Popups.UpdateTimerPopup>(PopupType.UpdateTimer);

					popup.OnConfirm = () =>
					{
						UpdateApp();
					};

				}

#endif

			}
			else
			{
				it.Main.PopupController.Instance.ShowPopup<it.Popups.InfoPopup>(PopupType.Info).SetDescriptionString(string.Format("popup.update.appUpdateReady".Localized(), STEP_TO_EXIT));
#if UNITY_STANDALONE
				PlayerPrefs.SetInt(StringConstants.APP_UPDATE, 1);
#endif
			}

			//EmitExistsUpdate();

			//if (isStart)
			//{
			//	it.Main.PopupController.Instance.ShowPopup(PopupType.Update);
			//}
		}

		private void EmitExistsUpdate()
		{
			//it.Logger.Log("[UPDATER] Exists criticle update");
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.AppExistsUpdate, 0.01f);
		}

		public void UpdateApp()
		{
#if UNITY_ANDROID
			if (_maxVeraionToUpdate != null)
			{
				Application.OpenURL(_maxVeraionToUpdate.download_url);
				//it.Logger.Log($"[UPDATER] open URL {_maxVeraionToUpdate.download_url}");
			}		

#elif UNITY_WEBGL
			reloadPage();
#elif UNITY_STANDALONE

			StandaloneController.CloseAllTable();
			StandaloneController.Instance.CloseApplication();

			if (_launcherPath != null)
				StandaloneController.Instance.OpenApp(_launcherPath);
#endif

			//it.Logger.Log("[UPDATER] UpdateApp");
		}

		public void ExitApp()
		{
			Application.Quit();
			//it.Logger.Log("[UPDATER] ExitApp");
		}
#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void reloadPage();
#endif

	}
}