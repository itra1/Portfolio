using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Managers.Libraries;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Addressable;
using Game.Scripts.Providers.Saves;
using Game.Scripts.Providers.Songs.Saves;
using Game.Scripts.Providers.Songs.Settings;
using UnityEngine;

namespace Game.Scripts.Providers.Songs
{
	public class SongsProvider : ISongsProvider
	{
		private readonly IAddressableProvider _addressableProvider;
		private readonly ISongsAddressables _addressables;
		private readonly ISongsNoCover _noCover;
		private readonly ISaveProvider _saveProvider;
		private readonly ISongsResources _resourcesSettings;
		private SongsSave _songSave;

		public List<RhythmTimelineAsset> Songs { get; private set; } = new();

		public SongsProvider(
			IAddressableProvider addressableProvider,
			ISaveProvider saveProvider,
			ISongsAddressables addressables,
			ISongsNoCover noCover,
			ISongsResources resourcesSettings
		)
		{
			_addressableProvider = addressableProvider;
			_addressables = addressables;
			_noCover = noCover;
			_saveProvider = saveProvider;
			_resourcesSettings = resourcesSettings;
		}

		public async UniTask StartAppLoad(IProgress<float> onProgress, CancellationToken cancellationToken)
		{
			_songSave = _saveProvider.GetProperty<SongsSaveData>().Value;
#if UNITY_EDITOR && !ADDRESSABLE_SONGS_EDITOR
			LoadEditorFromAssetDatabase();
#else
			await LoadAddressableResources(onProgress, cancellationToken);
#endif
			await UniTask.Yield();

		}

		private async UniTask LoadAddressableResources(IProgress<float> onProgress, CancellationToken cancellationToken)
		{
			float progress = 0;
			float progressIncrement = 1f / _addressables.AddressableLibs.Length;

			IProgress<float> progressReporter = new Progress<float>(progressValue =>
			{
				onProgress?.Report(progress + (progressValue * progressIncrement));
			});

			foreach (var item in _addressables.AddressableLibs)
			{
				try
				{
					var baseLibrary = await _addressableProvider.LoadAssetAsync<TimelinesLibrary>(item, progressReporter, cancellationToken);
					if (baseLibrary != null)
						Songs.AddRange(baseLibrary.TimelinesList);
					progress += progressIncrement;
					onProgress?.Report(progress);
				}
				catch (Exception)
				{
					AppLog.LogError($"SongsProvider error load {item}");
				}
			}

			AppLog.Log($"Songs count {Songs.Count}");
			onProgress?.Report(1);
		}

		private void LoadResources()
		{
			Songs = (from p in Resources.LoadAll<RhythmTimelineAsset>(_resourcesSettings.ResourcesPath)
							 orderby p.ConditionStar, p.Authour
							 select p).ToList();
		}
#if UNITY_EDITOR
		private void LoadEditorFromAssetDatabase()
		{
			var assets = UnityEditor.AssetDatabase.LoadAssetAtPath<TimelinesLibrary>("Assets/Resources_moved/BaseTimelinesLibrary.asset");

			Songs.AddRange(assets.TimelinesList);
			assets = UnityEditor.AssetDatabase.LoadAssetAtPath<TimelinesLibrary>("Assets/Resources_moved/TestTimelinesLibrary.asset");

			Songs.AddRange(assets.TimelinesList);
		}
#endif

		public RhythmTimelineAsset GetSong(string uuid)
			=> Songs.Find(x => x.Uuid == uuid);

		public Texture2D GetCover(string songUuid)
		{
			var song = GetSong(songUuid);

			return song == null || song.Cover == null ? _noCover.NoImageCover : song.Cover;
		}
	}
}
