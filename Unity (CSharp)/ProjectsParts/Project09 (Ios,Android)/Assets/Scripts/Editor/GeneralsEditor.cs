using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;

public class GeneralsEditor : EditorWindow {

  private static string filePath = "Text/config.json.txt";
  
  public int mainToolBar = 0;
  public string[] mainToolBarStrings = new string[] {"Config", "Plugins"};

  [MenuItem("Generals/Config")]
  static void Init() {
    EditorWindow window = GetWindow(typeof(GeneralsEditor));
    window.position = new Rect(400, 400, 500, 360);
    window.Show();
  }

  void OnGUI() {

    if(!init || rootNode == null) {
      init = true;
      LoadData();
      InitPlugins();
    }


    EditorGUILayout.BeginVertical();
    mainToolBar = GUILayout.Toolbar(mainToolBar, mainToolBarStrings);
    EditorGUILayout.EndHorizontal();

    switch(mainToolBar) {
      case 0:
        ConfigShow();
        break;
      case 1:
        DrawPlugins();
        break;
    }

  }

  #region Параметры

  private string newNodeName="NewNode";
  private string newFieldName="NewField";
  private bool toAllChilds=false;
  private Vector2 scrollPos;

  private bool init=false;
  private JSONNode rootNode;

  private Dictionary<JSONNode, bool> opened = new Dictionary<JSONNode, bool>();
  private JSONNode focused = null;
  private JSONNode focusedParent=null;


  void LoadData() {
    string path = Application.dataPath + "/" + filePath;
    try {
      StreamReader sr = new StreamReader(path);
      string fileContents = sr.ReadToEnd();
      sr.Close();
      rootNode = JSON.Parse(fileContents);
      //SimpleJSON.JSONClass.LoadFromStream

    } catch(IOException) {
      //System.IO.Directory.CreateDirectory ((new System.IO.FileInfo (path)).Directory.FullName);
      //System.IO.File.WriteAllText(path, "{}");

      rootNode = JSON.Parse("{}");
    }

    //SaveConfig();
  }


  private void ConfigShow() {


    EditorGUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Reload")) {
      LoadData();
      opened.Clear();
      init = true;
    }
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    newFieldName = GUILayout.TextField(newFieldName, GUILayout.Width(100));

    if(GUILayout.Button("Add String")) {
      if(focused != null && focused.Tag == (JSONBinaryTag)0) {
        if(toAllChilds) {
          foreach(string key in focused.Keys) {
            if(focused[key].Tag == (JSONBinaryTag)0) {
              focused[key][newFieldName] = "";
              opened[focused[key][newFieldName]] = true;
            }
          }
        } else {
          focused[newFieldName] = "";
          opened[focused] = true;
        }
        toAllChilds = false;
      }
    }

    if(GUILayout.Button("Add Number")) {
      if(focused != null && focused.Tag == (JSONBinaryTag)0) {
        if(toAllChilds) {
          foreach(string key in focused.Keys) {
            if(focused[key].Tag == (JSONBinaryTag)0) {
              focused[key][newFieldName].AsFloat = 0f;
              opened[focused[key][newFieldName]] = true;
            }
          }
        } else {
          focused[newFieldName].AsFloat = 0f;
          opened[focused] = true;
        }
        toAllChilds = false;
      } else Debug.Log("focused == null || focused.Tag != (JSONBinaryTag)0 :" + focused + " " + (focused == null ? "null" : focused.Tag.ToString()));
    }
    GUILayout.FlexibleSpace();


    toAllChilds = EditorGUILayout.Toggle("To All Childs", toAllChilds);

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();

    newNodeName = GUILayout.TextField(newNodeName, GUILayout.Width(100));
    if(GUILayout.Button("Add Node")) {
      JSONClass newNode = new JSONClass();
      if(focused == null || (focusedParent == null && focused.Tag != (JSONBinaryTag)0)) {
        rootNode.Add(newNodeName, newNode);
      } else {
        JSONNode parent = focused.Tag==(JSONBinaryTag)0?focused:focusedParent;
        parent.Add(newNodeName, newNode);
        opened[parent] = true;
      }
    }
    EditorGUILayout.Space();
    GUILayout.FlexibleSpace();

    if(GUILayout.Button("Del Element")) {
      if(focused != null) {
        if(focusedParent != null)
          focusedParent.Remove(focused);
        else
          rootNode.Remove(focused);
      }
      focused = null;
    }

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.Space();
    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
    EditorGUILayout.Space();
    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
    EditorGUILayout.BeginVertical();
    //focused = null;
    ShowNode(rootNode);
    EditorGUILayout.Space();
    EditorGUILayout.EndVertical();
    EditorGUILayout.EndScrollView();
    GUILayout.FlexibleSpace();
    if(GUILayout.Button("Save")) {
      SaveConfig();
    }
  }

  void ShowNode(JSONNode nodeList) {
    if(nodeList == null)
      return;

    EditorGUILayout.BeginVertical();
    string newName="";
    JSONNode newNode=null;

    foreach(string key in nodeList.Keys) {
      JSONNode node = nodeList[key];
      if(!opened.ContainsKey(node))
        opened.Add(node, false);

      string controlName = GUIUtility.GetControlID(FocusType.Keyboard)+ToString();
      GUI.SetNextControlName(controlName);
      switch(node.Tag) {
        case (JSONBinaryTag)0: {
            //EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            opened[node] = EditorGUILayout.Foldout(opened[node], key);
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if(opened[node]) {
              EditorGUILayout.BeginHorizontal();
              EditorGUILayout.Space();
              EditorGUILayout.Space();
              ShowNode(node);
              EditorGUILayout.EndHorizontal();
            }

            break;
          }
        case JSONBinaryTag.Value: {
            EditorGUILayout.BeginHorizontal();
            string name = GUILayout.TextField(key, GUILayout.Width(130));
            if(name != key) {
              newName = name;
              newNode = node;
            }
            node.Value = EditorGUILayout.TextField(node.Value, GUILayout.Width(60));
            GUILayout.Label("(string)");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            break;
          }

        case JSONBinaryTag.IntValue:
        case JSONBinaryTag.DoubleValue:
        case JSONBinaryTag.FloatValue: {
            EditorGUILayout.BeginHorizontal();
            string name = GUILayout.TextField(key, GUILayout.Width(130));
            if(name != key) {
              newName = name;
              newNode = node;
            }
            node.AsFloat = EditorGUILayout.FloatField(node.AsFloat, GUILayout.Width(60));
            GUILayout.Label("(number)");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            break;
          }
      }
      if(GUI.GetNameOfFocusedControl() == controlName) {
        focused = node;
        focusedParent = nodeList;
        //Debug.Log(controlName + " "+key);
      } else {
        //Debug.Log(controlName);
      }

    }
    if(newNode != null) {
      nodeList.Remove(newNode);
      nodeList.Add(newName, newNode);
    }

    EditorGUILayout.EndVertical();
  }

  void OnInspectorUpdate() {
    this.Repaint();
  }

  void SaveConfig() {
    /*foreach(string key in rootNode["Enemies"].Keys){
			float speed = rootNode["Enemies"][key]["Speed"].AsFloat;
			//speed = speed*2.5f/Config.EnemyPathLength;
			//speed*=100;
			rootNode["Enemies"][key]["Speed"].AsFloat = speed;
		}*/

    //rootJson.SaveToBase64(Application.dataPath + "/" + filePath);
    string path = Application.dataPath + "/" + filePath;
    System.IO.File.WriteAllText(path, rootNode.ToJSON(0));
    //System.IO.File.WriteAllText(path, rootNode.SaveToBase64());

    AssetDatabase.Refresh();
  }

  #endregion
  
  #region Plugins

  struct PluginsParametrs {
    public string plugin;
    public bool ios;
    public bool android;
  }

  PluginsParametrs[] plugins;

  private readonly char[] defineSeperators = new char[] {';', ',', ' '};

  /// <summary>
  /// Инициализация плагина
  /// </summary>
  void InitPlugins() {

    string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
    string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);

    List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
    List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);

    plugins = new PluginsParametrs[1];
    plugins[0].plugin = "PLUGIN_FACEBOOK";

    for(int i = 0; i < plugins.Length; i++) {
      plugins[i].ios = _newDefineSymbolsIOS.Contains(plugins[i].plugin) ? true : false;
      plugins[i].android = _newDefineSymbolsAndroid.Contains(plugins[i].plugin) ? true : false;
    }

  }

  void DrawPlugins() {

    EditorGUILayout.BeginHorizontal();
    GUILayout.Label("Plugin", GUILayout.Width(150));
    GUILayout.FlexibleSpace();
    GUILayout.Label("IOS", GUILayout.Width(150));
    GUILayout.FlexibleSpace();
    GUILayout.Label("Android", GUILayout.Width(150));
    GUILayout.FlexibleSpace();
    EditorGUILayout.EndHorizontal();

    for(int i = 0; i < plugins.Length; i++) {
      EditorGUILayout.BeginHorizontal();
      GUILayout.Label(plugins[i].plugin, GUILayout.Width(150));
      GUILayout.FlexibleSpace();
      plugins[i].ios = EditorGUILayout.Toggle(plugins[i].ios);
      GUILayout.FlexibleSpace();
      plugins[i].android = EditorGUILayout.Toggle(plugins[i].android);
      GUILayout.FlexibleSpace();
      EditorGUILayout.EndHorizontal();
    }


    EditorGUILayout.BeginHorizontal();
    if(GUILayout.Button("Save")) {
      SavePlugins();
    }
    GUILayout.FlexibleSpace();
    EditorGUILayout.EndHorizontal();
  }

  void SavePlugins() {

    string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);

    string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);

    List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
    List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);

    for(int i = 0; i < plugins.Length; i++) {
      AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsIOS, plugins[i].ios, plugins[i].plugin);
      AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsAndroid, plugins[i].android, plugins[i].plugin);
    }

    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, string.Join(";", _newDefineSymbolsIOS.ToArray()));
    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", _newDefineSymbolsAndroid.ToArray()));

  }

  private void AddOrRemoveFeatureDefineSymbol(List<string> _defineSymbols, bool _usesFeature, string _featureSymbol) {
    if(_usesFeature) {
      if(!_defineSymbols.Contains(_featureSymbol))
        _defineSymbols.Add(_featureSymbol);
    } else {
      if(_defineSymbols.Contains(_featureSymbol))
        _defineSymbols.Remove(_featureSymbol);
    }
  }

  #endregion


}