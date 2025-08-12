using Core.Engine.Components.Themes.Common;
using Scripts.Elements.Themes;
using UnityEngine;

namespace Scripts.Common {
	public class SceneTheme : Theme {
		[SerializeField] private bool _isDefault;
		[SerializeField, Uuid.UUID] private string _uuid;
		[SerializeField] private string _title;

		[SerializeField, ColorUsage(false, false)] private Color _wallColor;
		[SerializeField, ColorUsage(false, false)] private Color _wallShadowColor;
		[SerializeField] private Vector2 _wallBackAngle = new Vector2(-0.8f, 0.8f);
		[SerializeField, ColorUsage(false, false)] private Color _coloneColor;
		[SerializeField, ColorUsage(false, false)] private Color _platformColor;
		[SerializeField, ColorUsage(false, false)] private Color _platformDamageColor;
		[SerializeField, Range(-1f, 1f)] private float _platformShadow = 0.2f;
		[SerializeField, ColorUsage(false, false)] private Color _ball;

		private SceneComponents _sceneComponents;

		public override string UUID => _uuid;
		public override bool IsDefault => _isDefault;
		public float PlatformShadow => _platformShadow;

		public Color WallColor => _wallColor;
		public Color WallShadowColor => _wallShadowColor;
		public Color ColoneColor => _coloneColor;
		public Color PlatformColor => _platformColor;
		public Color PlatformDamageColor => _platformDamageColor;
		public override Color Ball => _ball;

		public SceneTheme(SceneComponents sceneComponents) {
			_sceneComponents = sceneComponents;
		}

		public override bool Confirm(IThemeComponents themeComponents) {
			if (themeComponents is not GameThemeComponents gametheme)
				return false;

			var wallmaterial = gametheme.WallMaterial;
			var platformMaterial = gametheme.PlatformMaterial;
			var platformDamageMaterial = gametheme.PlatformDamageMaterial;
			var coloneMaterial = gametheme.ColoneMaterial;
			try {
				wallmaterial.SetColor("_ColorOne", _wallColor);
				wallmaterial.SetColor("_ColorTwo", _wallShadowColor);
				wallmaterial.SetFloat("_RotateGradient", UnityEngine.Random.Range(-0.2f, 0.2f));

				platformMaterial.SetColor("_Color", _platformColor);
				platformDamageMaterial.SetColor("_Color", _platformDamageColor);
				platformMaterial.SetFloat("_Shadow", _platformShadow);

				coloneMaterial.SetColor("_Color", _coloneColor);

				return true;
			} catch {
				return false;
			}
		}

#if UNITY_EDITOR

		[SerializeField] private Renderer _wall;
		[SerializeField] private Renderer _platform;
		[SerializeField] private Renderer _colone;

		[ContextMenu("Read Colors")]
		public void ReadColors() {

			_wallColor = _wall.sharedMaterial.GetColor("_ColorOne");
			_wallShadowColor = _wall.sharedMaterial.GetColor("_ColorTwo");

			_platformColor = _platform.sharedMaterial.GetColor("_BaseColor");
			//_platformEmissionColor = _platform.sharedMaterial.GetColor("_Emission");

			_coloneColor = _colone.sharedMaterial.GetColor("_BaseColor");
			//_coloneEmissionColor = _colone.sharedMaterial.GetColor("_Emission");

		}

#endif

	}
}
