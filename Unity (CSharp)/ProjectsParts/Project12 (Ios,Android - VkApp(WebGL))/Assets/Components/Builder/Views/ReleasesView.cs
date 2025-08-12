using System.Collections.Generic;
using System.IO;
using System.Linq;
using Builder.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Zenject;

namespace Builder.Views
{
	public class ReleasesView : ViewBase
	{
		private readonly BuilderWindow _window;

		private bool _loadingResources;
		private ScrollView _scrollView;
		private static List<Release> _resourcesList = new();
		private VisualTreeAsset _recordPrefab;

		private Button _getResourcesButton;

		public override string Type => ViewsType.Releases;

		public ReleasesView(BuildSession data) : base(data)
		{
			_ = StaticContext.Container;
		}

		protected override void LoadPrefab()
		{
			_viewPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuilderWindow.ReleasesViewTemplate);
			_recordPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuilderWindow.BuildRecordTemplate);
		}

		protected override void CreateUi()
		{
			base.CreateUi();

			_getResourcesButton = _view.Q<Button>("getResourcesButton");
			_getResourcesButton.clicked += LoadDataAndSpawn;
			_scrollView ??= _view.Q<ScrollView>("releaseListView");

			_buildData.BodyElement.Add(_view);
		}

		public override void Show()
		{
			base.Show();

			ReadLocal();

			if (_resourcesList.Count > 0)
				SpawnItems();
			else
				LoadDataAndSpawn();
		}

		private void LoadDataAndSpawn()
		{
			LoadData(result =>
			{
				if (result)
					SpawnItems();
			});
		}

		private void LoadData(UnityAction<bool> onFinish)
		{
			//LoadResourcesData((result) =>
			//{
			//	if (!result)
			//		error = "Ошибка загрузки ресурсов";

			//	isWait = false;
			//});

			SpawnItems();
		}

		private void ReadLocal()
		{
			if (Directory.Exists(_buildData.Settings.ArchivePath))
				ReadLocalPath(_buildData.Settings.ArchivePath);
		}

		private void ReadLocalPath(string path)
		{
			var files = Directory.GetFiles(path);

			files = files
				.ToList()
				.OrderByDescending(x =>
				{
					FileInfo fi = new(x);
					var namePointSplit = fi.Name.Split('.');
					return namePointSplit[^1];
				})
				.ToArray();

			for (var i = 0; i < files.Length; i++)
			{
				var fullPath = files[i];

				FileInfo fi = new(fullPath);

				var namePointSplit = fi.Name.Split(".");
				var type = namePointSplit[^1];

				if (type != "zip" && type != "txt")
					continue;

				var isTxt = type == "txt";
				var fName = fi.Name.Remove(fi.Name.Length - 4);
				var fiBlocks = fName.Split('_');

				if (fiBlocks.Length <= 1)
					continue;

				var platform = fiBlocks[0];
				var version = fiBlocks[1];

				var record = _resourcesList.Find(x => x.Version == version);

				if (isTxt && record == null)
					continue;

				if (record != null)
				{
					//record.IsChangeDescription = false;

					if (!isTxt)
					{
						//record.FilePath = fullPath;
						record.FileName = fi.Name;
					}
					else
					{
						try
						{
							//TODO Проработать момент асинхронного обращения
							var localDesc = File.ReadAllText(fullPath);

							if (localDesc != record.Description)
							{
								record.Description = localDesc;
								//record.IsChangeDescription = true;
							}
						}
						catch
						{
							// ignored
						}
					}

					continue;
				}

				record = new Release
				{
					Version = version,
					Platform = platform,
					//Type = isSa ? ReleaseMaterialType.StreamingAssets : name,
					//FilePath = fullPath,
					//CreatedAt = fi.CreationTime.ToString(CultureInfo.InvariantCulture),
					FileName = fi.Name,
				};

				_resourcesList.Add(record);
			}
			Debug.Log($"Records count {_resourcesList.Count}");
		}

		private void SpawnItems()
		{
			if (_scrollView == null)
				return;

			_scrollView.Clear();

			_resourcesList = _resourcesList.OrderByDescending(x => x.CreateTime).ToList();

			foreach (var item in _resourcesList)
			{
				ReleaseItemView r = new(item, _window)
				{
					//OnChangeUploadState = () => LoadResourcesData(_ => SpawnItems())
				};

				_scrollView.Add(r.SpawnItem(_recordPrefab));
			}

			_scrollView.style.height = 0;
		}

		//private void GetResources()
		//{
		//	if (_loadingResources)
		//		return;

		//	_loadingResources = true;

		//	if (!string.IsNullOrEmpty(_window.Token))
		//	{
		//		LoadResourcesData();
		//	}
		//	else
		//	{
		//		Authorization(() => LoadResourcesData(),
		//			error => Debug.Log($"Authorization error {error}"));
		//	}
		//}

		//private void LoadResourcesData(UnityAction<bool> OnFinish = null)
		//{
		//	var url = $"{_window.ServerApi}/releases";

		//	_window.WebRequest(HTTPMethods.Get, url, null, "", null, (isComplete, resut) =>
		//	{
		//		if (!isComplete)
		//		{
		//			Debug.LogError($"Error {resut} | Request: {url}");
		//			return;
		//		}

		//		Debug.Log(resut);

		//		_loadingResources = false;

		//		_resourcesList.Clear();

		//		var jRes = JArray.ParseString(resut);

		//		for (var i = 0; i < jRes.Length; i++)
		//		{
		//			var el = _parsingHelper.Parse<Release>(jRes.GetJSON(i));

		//			if (el == null)
		//				continue;

		//			el.IsServer = true;

		//			_resourcesList.Add(el);
		//		}

		//		_resourcesList = _resourcesList.OrderByDescending(x => x.CreateTime).ToList();

		//		OnFinish?.Invoke(true);
		//	});
		//}
	}
}