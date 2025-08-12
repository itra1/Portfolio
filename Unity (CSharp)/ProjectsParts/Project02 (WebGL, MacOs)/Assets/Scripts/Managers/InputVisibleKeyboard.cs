using System.Collections;

using TMPro;

using UnityEngine;

public class InputVisibleKeyboard : MonoBehaviour
{
#if UNITY_WEBGL
	private TMP_InputField _inputfield;

	private void Start()
	{
		_inputfield = GetComponent<TMP_InputField>();
		_inputfield.onSelect.AddListener((str) =>
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			VisibleKeyboard.Instance.Visible(_inputfield);
#endif
		});
	}
#endif
}