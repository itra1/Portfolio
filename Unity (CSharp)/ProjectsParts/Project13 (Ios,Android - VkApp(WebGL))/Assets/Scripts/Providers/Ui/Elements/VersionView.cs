using TMPro;
using UnityEngine;

namespace Game.Providers.Ui.Elements {
	public class VersionView :MonoBehaviour {
		[SerializeField] private TMP_Text _versionLabel;

		public void OnEnable() {
			PrintVersion();
		}

		[ContextMenu("Print")]
		private void PrintVersion() {
			var version = Application.version;
			_versionLabel.text = $"Ver. {version}";
		}
	}
}
