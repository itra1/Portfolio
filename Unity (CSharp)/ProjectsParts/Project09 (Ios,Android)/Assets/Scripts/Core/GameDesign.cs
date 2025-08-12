using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameDesign))]
public class GameDesignEditor : Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
  }
}

#endif

[System.Serializable]
public struct CardLibrary {
  public string name;
  public string id;
  public string code;
  public string description;
  public int energyCost;
  public int level;
  public Dictionary<string, string> parameters;
}

/// <summary>
/// Общие настройки
/// </summary>
public class GameDesign : MonoBehaviour {

  public static GameDesign instance;
  
  void Awake() {

    if(instance != null) {
      Destroy(this);
      return;
    }
    instance = this;
  }

  public void Start() { }

	#region Оружие
	
	public List<GameObject> weaponManagers;        // Список префабов

	#endregion

	#region Карты игрока

	public List<CardLibrary> cardList;
  public List<CardInfo> cardLibrary;                      // Библиотека доступных карт
  public List<WeaponParams> weaponCardParam;                  // Соотнощение доступных карт

  public void CardParsing(List<CardLibrary> cardData) {
    cardList = cardData;
  }

  #endregion
  
}
