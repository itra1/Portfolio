using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Editor.Building
{
  public class Archivate
	{
		private BuildWindow _window;
		private string StreamingAssetsPath => _window.BuildPath + "/VideoWall_Data/StreamingAssets";
		private string GetArchivePath => _window.BuildPath + "\\..";

		public Archivate(BuildWindow window)
		{
			_window = window;
		}
		[MenuItem("Build/Compress", false, 0)]
		public void Compress()
		{
			if (_window.ArchivateStreamingAssets)
				CompressionStreamingAssets(() =>
				{
					ComplessionBase();
				});
			else
				ComplessionBase();
		}

		public async void CompressionStreamingAssets(UnityAction onComplete)
		{

			var obs = Observable.FromCoroutine<string>((observer, cancellationToken) => ComlessionStreamingAssetsProcess(observer, cancellationToken));
			await obs;
			Debug.Log("Complete archive streamin assets");
			onComplete?.Invoke();
			//obs.Subscribe(_ =>
			//{
			//	onComplete?.Invoke();
			//	Debug.Log("Complete archive streamin assets");
			//});
		}

		private void RemoveStreamingAssets()
		{
			if (Directory.Exists(StreamingAssetsPath))
				Directory.Delete(StreamingAssetsPath, true);
		}

		public async void ComplessionBase()
		{

			var obs = Observable.FromCoroutine<string>((observer, cancellationToken) => CompressorProcess(observer, cancellationToken));
			await obs;
			Debug.Log("Complete archive base");
			//obs.Subscribe(_ =>
			//{
			//	Debug.Log("Complete archive base");
			//});
		}

		private IEnumerator ComlessionStreamingAssetsProcess(IObserver<string> observer, CancellationToken cancellationToken)
		{
			//Init();
			yield return null;
			string filezip = GetNameArchive(true, _window.ArchivateStreamingAssets ? true : false);

			ZipFile.CreateFromDirectory(StreamingAssetsPath, GetArchivePath + "\\" + filezip, CompressionLevel.Optimal, false);
			//lzip.compressDir(StreamingAssetsPath, 9, GetArchivePath + "\\" + filezip, false);

			observer.OnNext("");
			observer.OnCompleted();
		}

		private IEnumerator CompressorProcess(IObserver<string> observer, CancellationToken cancellationToken)
		{
			//Init();

			if (_window.ArchivateRemoveStreaminAsset)
				RemoveStreamingAssets();

			yield return null;

			string filezip = GetNameArchive(false, _window.ArchivateStreamingAssets ? false : true);

			ZipFile.CreateFromDirectory(_window.BuildPath, GetArchivePath + "\\" + filezip, CompressionLevel.Optimal,false);

			//lzip.compressDir(_window.BuildPath, 9, GetArchivePath + "\\" + filezip, false);

			observer.OnNext("");
			observer.OnCompleted();

		}


		private string GetNameArchive(bool streaminAssets = false, bool increment = true)
		{
			int build = 0;
			string version = "";
			if (EditorPrefs.HasKey("Build_V"))
			{
				version = EditorPrefs.GetString("Build_V");
				//build = EditorPrefs.GetInt("Build_B");
				//if (increment)
				//	build++;
			}

			if (version != PlayerSettings.bundleVersion)
			{
				version = PlayerSettings.bundleVersion;
				build = 0;
			}

			EditorPrefs.SetString("Build_V", version);
			EditorPrefs.SetInt("Build_B", build);
			return string.Format("VideoWall_{0}{1}.zip", PlayerSettings.bundleVersion, (streaminAssets ? "_SA" : ""));
		}


	}
}