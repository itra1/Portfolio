using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Garilla.Games.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(HandHistoryPopup))]
public class HandHistoryPopupEditor : Editor
{
	private int _tableId;
	private string _startPeriod;
	private string _endPeriod;
	private bool _visibleFold = false;

	private void OnEnable()
	{
		_tableId = 0;
	}

	public override void OnInspectorGUI()
	{
		_visibleFold = EditorGUILayout.Foldout(_visibleFold, "Show Form");
		if (_visibleFold)
		{
			_tableId = EditorGUILayout.IntField("Table Id", _tableId);
			_startPeriod = EditorGUILayout.TextField("Start period", _startPeriod);
			_endPeriod = EditorGUILayout.TextField("End period", _endPeriod);
			if (GUILayout.Button("Get"))
			{

				if (_tableId <= 0 || string.IsNullOrEmpty(_startPeriod) || string.IsNullOrEmpty(_endPeriod))
					return;

				((HandHistoryPopup)target).GetCustomHistory((ulong)_tableId, _startPeriod, _endPeriod);

			}
		}
		EditorGUILayout.Separator();
		base.OnInspectorGUI();
	}

}
