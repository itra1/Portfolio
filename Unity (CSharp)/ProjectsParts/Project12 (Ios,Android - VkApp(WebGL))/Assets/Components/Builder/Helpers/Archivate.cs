using System.IO;
using Builder.Common;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;

namespace Builder
{
	public class Archivate
	{
		private BuildSession _buildData;

		public Archivate(BuildSession buildData)
		{
			_buildData = buildData;
		}

		public void Compress()
		{
			string folderToCompress = $"{_buildData.Window.BuildPath}";
			var fileName = $"{_buildData.Platform}_{_buildData.Version}.zip";
			string zipPath = Path.Combine(_buildData.Settings.ArchivePath, fileName);
			Debug.Log($"Compress {zipPath} name {folderToCompress}");

			ZipUtility.CompressFolderToZip(zipPath, null, folderToCompress);
		}

		private string GetNameArchive(bool increment = true)
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
			return string.Format("VideoWall_{0}.zip", PlayerSettings.bundleVersion);
		}
	}
}