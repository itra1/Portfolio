using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogConfirm : EditorWindow {

	public Action OnConfirm;
	public Action OnCancel;

	public string textInfo;

	public void OnEnable() {
		this.titleContent.text = "Подтверждение";

		this.minSize = new Vector2(300, 20);
	}

	private void OnGUI() {
		GUILayout.BeginHorizontal();
		GUILayout.Label(textInfo);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("OK")) {
			if (OnConfirm != null) OnConfirm();
			this.Close();
		}

		if (GUILayout.Button("Отмена")) {
			if (OnCancel != null) OnCancel();
			this.Close();
		}

		GUILayout.EndHorizontal();
	}

	public static void ShowDialog(string textDialog, Action OnConfirm, Action OnCancel) {
		DialogConfirm dialog = (DialogConfirm)EditorWindow.GetWindow(typeof(DialogConfirm));
		dialog.textInfo = textDialog;
		dialog.OnConfirm = OnConfirm;
		dialog.OnCancel = OnCancel;
	}
}
