using TMPro;
using UnityEngine;

namespace Ugui {
	public class AppVersion : MonoBehaviour {
		[SerializeField] private TMP_Text _label;

		private void Awake() {
			_label.text = Application.version;
		}
	}
}
