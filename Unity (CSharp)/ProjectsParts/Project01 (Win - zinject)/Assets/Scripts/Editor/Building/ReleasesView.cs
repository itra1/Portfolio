using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BestHTTP;
using Core.Materials.Parsing;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Options;
using Cysharp.Threading.Tasks;
using Editor.Build.Materials;
using Editor.Build.Materials.Data;
using Leguar.TotalJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Zenject;

namespace Editor.Building
{
	public class ReleasesView
	{
		private readonly IApplicationOptionsSetter _optionsSetter;
		private readonly IMaterialDataParsingHelper _parsingHelper;
		private readonly IHttpRequest _request;
		private readonly BuildWindow _window;
		private readonly VisualTreeAsset _itemPrefab;
		
		private VisualElement _root;
		private bool _loadingResources;
		private ScrollView _scrollView;
		private static List<ReleaseMaterialData> _resourcesList = new();

		public ReleasesView(BuildWindow window)
		{
			var container = StaticContext.Container;
			
			_optionsSetter = container.TryResolve<IApplicationOptionsSetter>();
			_parsingHelper = container.TryResolve<IMaterialDataParsingHelper>();
			_request = container.TryResolve<IHttpRequest>();

			_window = window;
			_itemPrefab = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/Building/BuildRecord.uxml");
			_root = window.Root;
		}

		public void Draw()
		{
			_root ??= _window.Root;
			
			if (_root == null) 
				return;

			_scrollView ??= _root.Q<ScrollView>("releaseListView");
			
			var resourceButton = _root.Q<Button>("getResourcesButton");
			
			resourceButton.clicked += LoadDataAndSpawn;

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
			}).Forget();
		}

		private async UniTaskVoid LoadData(UnityAction<bool> onFinish)
		{
			var isWait = false;
			var error = string.Empty;

			if (string.IsNullOrEmpty(_window.Token))
			{
				isWait = true;
				
				Authorization(() => 
					{
						isWait = false;
					},
					err =>
					{
						error = err;
						isWait = false;
					});
				
				await UniTask.WaitUntil(() => isWait == false);
				
				if (!string.IsNullOrEmpty(error))
				{
					onFinish?.Invoke(false);
					return;
				}
			}
			
			isWait = true;
			
			LoadResourcesData((result) =>
			{
				if (!result)
					error = "Ошибка загрузки ресурсов";
				
				isWait = false;
			});
			
			await UniTask.WaitUntil(() => isWait == false);
			
			SpawnItems();
		}

		private void ReadLocal()
		{
			if (Directory.Exists(_window.BuildPath + @"\.."))
				ReadLocalPath(_window.BuildPath + @"\..");
			
			if (Directory.Exists(_window.BuildPath + @"\..\..\..\Builds"))
				ReadLocalPath(_window.BuildPath + @"\..\..\..\Builds");
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
				
				var name = fiBlocks[0];
				var version = fiBlocks[1];
				
				var isSa = fiBlocks.Length >= 3 && fiBlocks[2] == "SA";

				if (string.Equals(name, ReleaseMaterialType.VideoWall, StringComparison.CurrentCultureIgnoreCase))
					name = ReleaseMaterialType.VideoWall;
				
				if (string.Equals(name, ReleaseMaterialType.StreamingAssets, StringComparison.CurrentCultureIgnoreCase))
					name = ReleaseMaterialType.StreamingAssets;
				
				if (string.Equals(name, ReleaseMaterialType.Launcher, StringComparison.CurrentCultureIgnoreCase))
					name = ReleaseMaterialType.Launcher;

				var record = _resourcesList.Find(x => x.Version == version && (isSa ? x.Type == ReleaseMaterialType.StreamingAssets : x.Type == name));

				if (isTxt && record == null) continue;

				if (record != null)
				{
					record.IsChangeDescription = false;
					
					if (!isTxt)
					{
						record.IsLocal = true;
						record.FilePath = fullPath;
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
								record.IsChangeDescription = true;
							}
						}
						catch
						{
							// ignored
						}
					}

					continue;
				}
				
				record = new ReleaseMaterialData
				{
					Version = version,
					Type = isSa ? ReleaseMaterialType.StreamingAssets : name,
					IsLocal = true,
					FilePath = fullPath,
					FileName = fi.Name,
					CreatedAt = fi.CreationTime.ToString(CultureInfo.InvariantCulture)
				};
				
				_resourcesList.Add(record);
			}
		}

		private void SpawnItems()
		{
			if (_scrollView == null) return;

			_scrollView.Clear();

			ReadLocal();

			_resourcesList = _resourcesList.OrderByDescending(x => x.CreateTime).ToList();

			foreach (var item in _resourcesList)
			{
				ReleaseItem r = new (item, _window)
				{
					OnChangeUploadState = () => LoadResourcesData(_ => SpawnItems())
				};
				
				_scrollView.Add(r.SpawnItem(_itemPrefab));
			}
			
			_scrollView.style.height = 0;
		}

		private void Authorization(Action onComplete, Action<string> onFalse)
		{
			var url = $"{_window.ServerApi}{RestApiUrl.Login}";

			var parameters = new (string, object)[]
			{
				new ("username", _window.Login),
				new ("password", _window.Password),
				new ("role", _window.Role)
			};

			_request.Request(new Uri(url), 
				HttpMethodType.Post,
				parameters,
				null,
				result =>
				{
					var json = JSON.ParseString(result);
					
					_window.Token = json.GetString("token");
					
					Debug.Log($"Auth token {_window.Token}");
					
					_optionsSetter.ServerToken = _window.Token;
					
					onComplete?.Invoke();
				},
				error => 
				{
					Debug.LogError($"Login error {error} | Request: {url}");
					onFalse?.Invoke(error);
				});
		}
		
		private void GetResources()
		{
			if (_loadingResources)
				return;
			
			_loadingResources = true;
			
			if (!string.IsNullOrEmpty(_window.Token))
			{
				LoadResourcesData();
			}
			else
			{
				Authorization(() => LoadResourcesData(), 
					error => Debug.Log($"Authorization error {error}"));
			}
		}

		private void LoadResourcesData(UnityAction<bool> OnFinish = null)
		{
			var url = $"{_window.ServerApi}/releases";

			_window.WebRequest(HTTPMethods.Get, url, null, "", null, (isComplete, resut) =>
			{
				if (!isComplete)
				{
					Debug.LogError($"Error {resut} | Request: {url}");
					return;
				}

				Debug.Log(resut);
				
				_loadingResources = false;

				_resourcesList.Clear();

				var jRes = JArray.ParseString(resut);
				
				for (var i = 0; i < jRes.Length; i++)
				{
					var el = _parsingHelper.Parse<ReleaseMaterialData>(jRes.GetJSON(i));
					
					if (el == null)
						continue;
					
					el.IsServer = true;
					
					_resourcesList.Add(el);
				}
				
				_resourcesList = _resourcesList.OrderByDescending(x => x.CreateTime).ToList();
				
				OnFinish?.Invoke(true);
			});
		}
	}
}