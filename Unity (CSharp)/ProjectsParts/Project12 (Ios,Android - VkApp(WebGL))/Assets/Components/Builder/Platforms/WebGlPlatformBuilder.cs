using System;
using Builder.Common;
using UnityEditor;
using UnityEditor.WebGL;
using UnityEngine.UIElements;

namespace Builder.Platforms
{
	public class WebGlPlatformBuilder : PlatformBuilder
	{
		public override string Name => "WebGL";

		public override BuildTarget BuildTarget => BuildTarget.WebGL;

		public override string FileName => "WebGL";

		public WebGlPlatformBuilder(BuildSession buildData) : base(buildData)
		{

		}

		public override void AddBuildOptions(VisualElement view, int index)
		{
			base.AddBuildOptions(view, index);

			var codeOptimization = new DropdownField("Code Optimization");

			codeOptimization.choices.Clear();
			codeOptimization.choices.Add(WasmCodeOptimization.RuntimeSpeed.ToString());
			codeOptimization.choices.Add(WasmCodeOptimization.RuntimeSpeedLTO.ToString());
			codeOptimization.choices.Add(WasmCodeOptimization.DiskSize.ToString());
			codeOptimization.choices.Add(WasmCodeOptimization.DiskSizeLTO.ToString());
			codeOptimization.choices.Add(WasmCodeOptimization.BuildTimes.ToString());

			codeOptimization.value = UserBuildSettings.codeOptimization.ToString();

			_ = codeOptimization.RegisterValueChangedCallback(value =>
			{
				UserBuildSettings.codeOptimization = Enum.Parse<WasmCodeOptimization>(value.newValue);
			});

			view.Insert(++index, codeOptimization);
		}
	}
}
