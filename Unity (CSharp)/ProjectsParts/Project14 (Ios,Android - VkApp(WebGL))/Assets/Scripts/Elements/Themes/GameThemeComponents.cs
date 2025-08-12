using Core.Engine.Components.Themes.Common;
using UnityEngine;

namespace Scripts.Elements.Themes {
	public class GameThemeComponents : ThemeComponents {
		[SerializeField] private Material _wallMaterial;
		[SerializeField] private Material _coloneMaterial;
		[SerializeField] private Material _platformMaterial;
		[SerializeField] private Material _platformDamageMaterial;

		public Material WallMaterial { get => _wallMaterial; set => _wallMaterial = value; }
		public Material ColoneMaterial { get => _coloneMaterial; set => _coloneMaterial = value; }
		public Material PlatformMaterial { get => _platformMaterial; set => _platformMaterial = value; }
		public Material PlatformDamageMaterial { get => _platformDamageMaterial; set => _platformDamageMaterial = value; }
	}
}
