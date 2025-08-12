using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ConchManager))]
public class ConchManagerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Add")) {
			((ConchManager)target).AddValue();
		}

		if (GUILayout.Button("Use")) {
			((ConchManager)target).Use();
		}


	}
}

#endif

/// <summary>
/// Менеджер ракушки
/// </summary>
public class ConchManager : Singleton<ConchManager> {

	public static event Action OnSetValue;  // Установка значения;
	public static event Action OnUse;

	private int _actualValue = 0;
	private bool _showHint = false;

	public int actualValue {
		get { return _actualValue; }
		set {
			_actualValue = value;
		}
	}

	public bool isFull { get { return actualValue >= 10; } }

	public void AddValue() {
		AddValue(1);
	}

	public void AddValue(int addValue) {
		SetValue(actualValue + addValue);
	}

	public void SetValue(int newVAlue) {
		actualValue = newVAlue;
		
		if (!_showHint) {
			_showHint = true;
		}

		if (OnSetValue != null) OnSetValue();
		Save();
	}

	public void Use() {
		//PlayerManager.Instance.coins += 500;
		SetValue(actualValue - 10);
		//Save();
		if (OnUse != null) OnUse();
	}

	public void Save() {

		PlayerPrefs.SetString("counch",
			JsonUtility.ToJson(new SaveData() {
				actualValue = _actualValue,
				showHint = _showHint
			})
		);

	}

	public void Load() {

		if (!PlayerPrefs.HasKey("counch")) return;

		SaveData save = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("counch"));

		_showHint = save.showHint;
		SetValue(save.actualValue);
		if (OnSetValue != null) OnSetValue();
	}

	[System.Serializable]
	public class SaveData {
		public int actualValue;
		public bool showHint;

	}

	public void ClickIcon() {

    if (GameManager.gamePhase == GamePhase.game && PlayerManager.Instance.company.GetActualSaveLevel().isComplited) return;

		ConchDialog cd = UIManager.Instance.GetPanel<ConchDialog>();
		cd.gameObject.SetActive(true);
		cd.Show();
	}

}
