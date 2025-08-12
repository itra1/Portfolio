using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Leguar.TotalJSON;

namespace it.Editor.TODO
{
  public class TODOWindow : EditorWindow
  {
	 private readonly string _editorPrefsKey = "TODOLIST";

	 private List<TODOItem> _todolist = new List<TODOItem>();

	 private void OnEnable()
	 {
		titleContent.text = "TODO Manager";
		Load();
	 }

	 [MenuItem("Tools/TODO")]
	 public static void Init()
	 {
		TODOWindow window = (TODOWindow)EditorWindow.GetWindow(typeof(TODOWindow));
		window.Show();
	 }
	 //[MenuItem("Tools/TODO/Delete")]
	 //public static void Delete()
	 //{
		//TODOWindow window = (TODOWindow)EditorWindow.GetWindow(typeof(TODOWindow));
		//window.Close();
	 //}

	 private class TODOItem
	 {
		public string Title;
		public string Description;
		public bool IsComplete;
		public Vector2 _descriptionScroll;
	 }

	 void OnGUI()
	 {
		GUILayout.Label("TODO Manager", EditorStyles.boldLabel);

		// Создание
		PrintCreateGUI();

		GUILayout.Label("List", EditorStyles.boldLabel);

		PrintList();
	 }

	 private void Save()
	 {
		JArray saveData = new JArray();

		for (int i = 0; i < _todolist.Count; i++)
		{
		  JSON saveItem = new JSON();
		  saveItem.Add("title", _todolist[i].Title);
		  saveItem.Add("description", _todolist[i].Description);
		  saveItem.Add("isComplete", _todolist[i].IsComplete);
		  saveData.Add(saveItem);
		}

		EditorPrefs.SetString(_editorPrefsKey, saveData.CreateString());
	 }

	 private void Load()
	 {
		if (!EditorPrefs.HasKey(_editorPrefsKey))
		  return;

		string data = EditorPrefs.GetString(_editorPrefsKey);

		Debug.Log(data);

		JArray _jList = JArray.ParseString(data);

		_todolist.Clear();

		for (int i = 0; i < _jList.Length; i++)
		{
		  JSON it = _jList.GetJSON(i);
		  TODOItem item = new TODOItem();
		  item.Title = it.GetString("title");
		  item.Description = it.GetString("description");
		  item.IsComplete = it.GetBool("isComplete");
		  _todolist.Add(item);
		}
	 }

	 [ContextMenu("Clear")]
	 public void ClearData()
	 {
		EditorPrefs.DeleteKey(_editorPrefsKey);
	 }

	 #region Print

	 private Vector2 _scroll;

	 private void PrintList()
	 {
		_scroll =EditorGUILayout.BeginScrollView(_scroll);
		EditorGUILayout.BeginVertical();

		List<TODOItem> tdl = _todolist.FindAll(x => !x.IsComplete);
		List<TODOItem> tdlc = _todolist.FindAll(x => x.IsComplete);

		PrintList(tdl, false);
		PrintList(tdlc, true);
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	 }

	 private void PrintList(List<TODOItem> todoList, bool isComplete)
	 {
		GUIStyle titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.normal.textColor = Color.white;

		for (int i = 0; i < todoList.Count; i++)
		{
		  GUI.backgroundColor = isComplete ? Color.green : Color.white;
		  EditorGUILayout.BeginHorizontal("box");

		  EditorGUILayout.BeginVertical();

		  EditorGUILayout.LabelField(todoList[i].Title, titleStyle);

		  GUILayout.TextArea(todoList[i].Description);

		  EditorGUILayout.EndVertical();

		  if (!isComplete)
		  {
			 GUI.backgroundColor = Color.green;
			 if (GUILayout.Button("V"))
			 {
				todoList[i].IsComplete = true;
				Save();
				Repaint();

			 }
		  }
		  else
		  {
			 GUI.backgroundColor = Color.yellow;
			 if (GUILayout.Button("^"))
			 {
				todoList[i].IsComplete = false;
				Save();
				Repaint();

			 }
		  }
		  GUI.backgroundColor = Color.red;
		  if (GUILayout.Button("X"))
		  {
			 todoList.Remove(todoList[i]);
			 Save();
			 Repaint();
		  }

		  EditorGUILayout.EndHorizontal();

		}
	 }

	 #endregion

	 #region Create

	 private string _inputCreateTitle;
	 private string _inputCreateDescription;
	 private bool _isCreate = false;

	 private void PrintCreateGUI()
	 {
		if (!_isCreate)
		{
		  if (GUILayout.Button("Create"))
		  {
			 _inputCreateTitle = "";
			 _inputCreateDescription = "";
			 _isCreate = true;
		  }
		  return;
		}

		PrintFormCreate();

	 }

	 private void PrintFormCreate()
	 {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Заголовок");
		_inputCreateTitle = EditorGUILayout.TextField(_inputCreateTitle);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Описание");
		_inputCreateDescription = EditorGUILayout.TextArea(_inputCreateDescription);
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
		  // Добавляем элемент
		  TODOItem newTodo = new TODOItem();
		  newTodo.Title = _inputCreateTitle;
		  newTodo.Description = _inputCreateDescription;
		  newTodo.IsComplete = false;
		  _todolist.Add(newTodo);

		  Save();

		  // Чистим
		  _inputCreateTitle = "";
		  _inputCreateDescription = "";
		  _isCreate = false;
		  Repaint();
		}
		if (GUILayout.Button("Cancel"))
		{
		  _inputCreateTitle = "";
		  _inputCreateDescription = "";
		  _isCreate = false;
		}
		EditorGUILayout.EndHorizontal();
	 }

	 #endregion

  }
}