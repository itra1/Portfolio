using UnityEngine;
using System.Collections;

/// <summary>
/// Менеджер интерфейсов
/// </summary>
public class UI : MonoBehaviour {

  public static UI instance;

  void Awake() {

    if(instance != null) {
      Destroy(this);
      return;
    }
    instance = this;

  }

  #region Меню

  public GameObject menuPanel;       // Префаб меню
  /// <summary>
  /// Показать меню
  /// </summary>
  /// <param name="isShow">Показать</param>
  /// <returns></returns>
  public GameObject MenuPanelShow(bool isShow) {
    menuPanel.SetActive(isShow);
    return menuPanel;
  }

  #endregion

  #region Окно авторизации

  public GameObject authorizationPanelPrefab;

  /// <summary>
  /// Отображение окна авторизации
  /// </summary>
  /// <param name="isShow">Видим</param>
  /// <returns>Созданный жкземпляр</returns>
  public GameObject AuthorizationPanelShow(bool isShow) {

    if(!isShow) return null;

    GameObject panelInst = Instantiate(authorizationPanelPrefab);
    panelInst.SetActive(isShow);
    panelInst.GetComponent<RectTransform>().SetParent(transform);
    panelInst.GetComponent<RectTransform>().localScale = Vector2.one;
    panelInst.GetComponent<RectTransform>().localPosition = Vector3.zero;
    return panelInst;
  }

  #endregion

  #region Игровой профиль
  public GameObject gameProfilePanelPrefab;

  public GameObject GameProfilePanelShow(bool isShow) {

    if(!isShow) return null;

    GameObject panelInst = Instantiate(gameProfilePanelPrefab);
    panelInst.SetActive(isShow);
    panelInst.GetComponent<RectTransform>().SetParent(transform);
    panelInst.GetComponent<RectTransform>().localScale = Vector2.one;
    panelInst.GetComponent<RectTransform>().localPosition = Vector3.zero;
    return panelInst;
  }

  #endregion

  #region Игровой профиль создание

  public GameObject gameProfileCreatePanelPrefab;

  public GameObject GameProfileCreatePanelShow(bool isShow) {

    if(!isShow) return null;

    GameObject panelInst = Instantiate(gameProfileCreatePanelPrefab);
    panelInst.SetActive(isShow);
    panelInst.GetComponent<RectTransform>().SetParent(transform);
    panelInst.GetComponent<RectTransform>().localScale = Vector2.one;
    panelInst.GetComponent<RectTransform>().localPosition = Vector3.zero;
    return panelInst;
  }

  #endregion

  #region Игровой персонаж

  public GameObject characterCreatePrefab;

  public GameObject CharacterCreatePanelShow(bool isShow) {

    if(!isShow) return null;

    GameObject panelInst = Instantiate(characterCreatePrefab);
    panelInst.SetActive(isShow);
    panelInst.GetComponent<RectTransform>().SetParent(transform);
    panelInst.GetComponent<RectTransform>().localScale = Vector2.one;
    panelInst.GetComponent<RectTransform>().localPosition = Vector3.zero;
    return panelInst;
  }

  #endregion

  #region Панель боя

  public GameObject battleMenu;

  public GameObject BattleMenuShow(bool isShow) {
    battleMenu.SetActive(isShow);
    return battleMenu;
  }

  #endregion

  #region Панель загрузки

  public GameObject loadingPanel;
  /// <summary>
  /// Показать панель загрузки
  /// </summary>
  /// <param name="isShow">Показать</param>
  /// <returns>Экземпляр панели</returns>
  public GameObject LoadingPanelShow(bool isShow) {
    loadingPanel.SetActive(isShow);
    return loadingPanel;
  }

  #endregion

  #region Информационная панель

  public GameObject infoPanel;

  public GameObject InfoPanelShow(string title, InfoPanel.ButtonGroup buttons, Actione<GameObject> OnOK = null, Actione<GameObject> OnCancel = null, Actione<GameObject> OnClose = null) {
    GameObject inst = Instantiate(infoPanel);
    inst.GetComponent<RectTransform>().SetParent(transform);
    inst.transform.localScale = Vector3.one;
    inst.transform.localPosition = Vector3.zero;
    inst.SetActive(true);
    inst.GetComponent<InfoPanel>().Init(title, buttons, OnOK, OnCancel, OnClose);
    return inst;
  }

  #endregion

}
