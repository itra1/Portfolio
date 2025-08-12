using System;
using System.Collections;
using System.IO;
using AddressablesManager.Runtime.Data;
using Ftp;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace AddressablesManager.Editor.Helpers
{
	public class CatalogHelper
	{
		private const string CatalogFile = "catalog.json";

		private CatalogData _catalog;
		private AddressableBuildWindow _window;
		private FtpClient _ftpClient;

		public CatalogHelper(AddressableBuildWindow window, FtpClient ftpClient)
		{
			_window = window;
			_ftpClient = ftpClient;
		}

		public IEnumerator LoadCatalog()
		{
			var catalog = "";

			var process = LoadCatalog((result) =>
			{
				catalog = result;
			});

			while (process.MoveNext())
				yield return null;

			_catalog = DeserializeCatalog(catalog);

		}

		public void MakeCatalog()
		{
			string profile = _window.AddressableSettings.OverridePlayerVersion;
			var currentCatalogProfile = _catalog.Builds.Find(x => x.Profile == _window.AddressableSettings.OverridePlayerVersion);

			_ = _catalog.Builds.Remove(currentCatalogProfile);

			currentCatalogProfile ??= new();

			currentCatalogProfile.CatalogFile = $"/{profile}/WebGL/catalog_{profile}.json";
			currentCatalogProfile.Profile = profile;
			currentCatalogProfile.UUID = Guid.NewGuid().ToString();

			_catalog.Builds.Add(currentCatalogProfile);

			RecordCatalog(_catalog);
		}

		public void UploadCatalog()
		{
			string filePath = $"{_window.LocalAddressablePath}/{CatalogFile}";
			string ftpPath = $"{_window.BuildSettings.Ftp.AddressablePath}/{CatalogFile}";
			_ftpClient.DeleteFile(ftpPath);
			_ftpClient.UploadFile(ftpPath, filePath);
		}

		public static IEnumerator LoadCatalog(UnityAction<string> callback)
		{
			var server = "https://netarchitect.ru/realgames/addressable_musictap";

			var request = UnityWebRequest.Get($"{server}/catalog.json");

			var process = request.SendWebRequest();

			while (!process.isDone)
				yield return null;

			var result = request.downloadHandler.text;

			request.Dispose();
			request = null;

			callback?.Invoke(result);
		}

		public string SerializeCatalog(CatalogData data)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(data);
		}
		public CatalogData DeserializeCatalog(string data)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<CatalogData>(data);
		}
		public void RecordCatalog(CatalogData data)
		{
			RecordCatalog(SerializeCatalog(data));
		}
		public void RecordCatalog(string catalog)
		{
			string path = $"{_window.LocalAddressablePath}/catalog.json";
			File.WriteAllText(path, catalog);
		}
	}
}
