using System;
using UnityEngine;
using Game.User;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DecorationManager))]
public class DecoretionEditor: Editor {
  private string mapNameFill;
  private int mapNumber;

  private DecorationManager script;

  private void OnEnable() {
    script = (DecorationManager)target;
  }



  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    EditorGUILayout.BeginHorizontal();
    mapNumber = EditorGUILayout.IntField("Имя карты:", mapNumber);

    if (GUILayout.Button("Установить декорации")) {
      script.LoadDecoration(mapNumber);
    }
    EditorGUILayout.EndHorizontal();

  }

}

#endif


/// <summary>
/// Свойства декораций сцены
/// </summary>
public class DecorationManager: Singleton<DecorationManager> {

  [SerializeField]
  private Transform decorations;                // Дирректория с декорациями

  public Location loaderLocation;

  private void Start() {

    int index = 1;
    Debug.Log(UserManager.Instance.ActiveBattleInfo.Mode);

    if (UserManager.Instance.ActiveBattleInfo != null)
      index = UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival || UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena
        ? UserManager.Instance.GetSurvivalFon()
        : GameDesign.Instance.allConfig.levels.Find(
            y => y.chapter == UserManager.Instance.ActiveBattleInfo.Group && y.level == UserManager.Instance.ActiveBattleInfo.Level).fon;

    index = Mathf.Max(1, index);
    LoadDecoration(index);
  }

  /// <summary>
  /// Установка необходимых декораций
  /// </summary>
  /// <param name="nameDecoration"></param>
  public void LoadDecoration(int mapNumber) {

    if(loaderLocation != null)
      DestroyImmediate(loaderLocation.gameObject);

    GameObject locPrefab = Resources.Load<GameObject>(String.Format("Map/{0}/Location", mapNumber));
    GameObject locInstance = Instantiate(locPrefab, decorations);
    loaderLocation = locInstance.GetComponent<Location>();
  }
  
}


