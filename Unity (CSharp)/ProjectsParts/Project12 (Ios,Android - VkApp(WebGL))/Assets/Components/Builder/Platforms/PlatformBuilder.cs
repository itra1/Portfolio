using Builder.Common;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder.Platforms
{
	public abstract class PlatformBuilder
	{
		static bool Building { get; set; }
		protected BuildSession _buildData;

		public abstract string Name { get; }
		public abstract string FileName { get; }
		public abstract BuildTarget BuildTarget { get; }

		protected PlatformBuilder(BuildSession buildData)
		{
			_buildData = buildData;
		}

		public virtual void OnPostprocessBuild(BuildReport report)
		{
			if (!Building)
				return;

			Debug.Log("OnPostprocessBuild");

			FileHelper.MoveFiles($"{_buildData.Window.BuildPath}/../{FileName}", $"{_buildData.Window.BuildPath}");

			Building = false;
		}

		public virtual void OnPreprocessBuild(BuildReport report)
		{

		}

		public void Build()
		{
			PreBuild();
			BuildProcess();
		}

		protected virtual void PreBuild(bool cleanCache = false) { }
		protected virtual void BuildProcess(bool cleanCache = false)
		{
			PlayerSettings.bundleVersion = _buildData.Version;
			Building = true;

			if (cleanCache)
				_ = BuildOptions.CleanBuildCache;

			_ = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, $"{_buildData.Window.BuildPath}/../{FileName}", BuildTarget, _buildData.BuildOptions);
		}

		public virtual void AddBuildOptions(VisualElement view, int index)
		{

		}
	}
}
