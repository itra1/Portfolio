using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;

/// <summary>
/// Специальная продажа
/// 
/// Раз в 24 часа показывать в течении 6 часов
/// 
/// </summary>
public class SpecialSales: Singleton<SpecialSales> {

  public static event Action<Phase> OnPhaseChange;

  public string specialProduct;                // Идентификатор специального продукта
  private DateTime _lastShow;
  private int _showCount = 0;

  public DateTime nextShow {
    get { return _lastShow.AddHours(24); }
  }

  private bool _isActive;
  private bool dialogOpen;

  private Phase _phase;
  public Phase phase {
    get { return _phase; }
    set {
      _phase = value;

      if (OnPhaseChange != null) OnPhaseChange(_phase);

    }
  }
  public enum Phase {
    none,
    wait,
    ready
  }

  private Scene _activeScene;

  void Start() {

    _isActive = false;

#if !UNITY_EDITOR
   // if(!BillingController.productDownload) return;
#endif
    // Запрет показа специального предложения ранее 3го запуска
    if (PlayerPrefs.GetInt("GameRun", 0) < 3) return;

    SceneManager.sceneLoaded += (scene, mode) => {

      _activeScene = scene;
      if (phase == Phase.ready)
        SetAciveIcon(true);
    };

    Load();
    Init();
  }

  IEnumerator WaitTimer() {

    while (nextShow > DateTime.Now) {
      yield return null;
    }

    Init();

  }

  private void Load() {

    SaveData saveData = null;

    if (PlayerPrefs.HasKey("specSale")) {
      saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("specSale"));
      _lastShow = DateTime.Parse(saveData.lastTime);
    } else {
      saveData = new SaveData() {
        count = 0
      };
      _lastShow = DateTime.Now;
    }
  }

  private void Save() {

    SaveData saveData = new SaveData() {
      lastTime = _lastShow.ToString(),
      count = _showCount
    };

    PlayerPrefs.SetString("specSale", Newtonsoft.Json.JsonConvert.SerializeObject(saveData));
  }

  private void Init() {

    int dayLast = (DateTime.Now - _lastShow).Hours / 24;

    if (dayLast > 0) {
      _showCount += dayLast;
      _lastShow = _lastShow.AddDays(dayLast);

      Save();
    }


    if ((nextShow - DateTime.Now).Hours < 6) {
      phase = Phase.ready;
    } else if ((nextShow - DateTime.Now).Hours >= 6) {
      phase = Phase.wait;
    }

  }

  void UpdateLastShow(bool now = false) {
    if (now) {
      PlayerPrefs.SetInt("specialSales", (int)(DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
    } else {
      //PlayerPrefs.SetInt("specialSales", lastSpecialSales);
    }
    SetActiveSpecialSales(false);
  }


  /// <summary>
  /// Событие активации окна
  /// </summary>
  public void IconButtonClick() {
    //dialogObject.SetActive(true);
    ShowSaleDialog();
  }

  void ShowSaleDialog() {

    SpecialSalesDialog saleDialog = UiController.ShowUi<SpecialSalesDialog>();
    saleDialog.gameObject.SetActive(true);

  }

  void ShowSaleProduct(Assets.Scripts.Billing.SpecialBillingProduct product) {
    SpecialSalesProduct saleDialog = UiController.ShowUi<SpecialSalesProduct>();
    saleDialog.gameObject.SetActive(true);
    //saleDialog.ByeSpecial();
    saleDialog.ShowByeSpecial(product);
  }

  /// <summary>
  /// Включаем специальное предложение
  /// </summary>
  /// <param name="flag"></param>
  void SetActiveSpecialSales(bool flag, bool fixedTime = false) {

    _isActive = flag;

    if (!flag) {
      SetAciveIcon(flag);
      //dialogObject.SetActive(flag);

      if (fixedTime) {
        PlayerPrefs.SetInt("specialSales", ((int)(DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds));
        return;
      }
    }

    SetAciveIcon(flag);
  }


  /// <summary>
  /// Иконка специальной покупки
  /// </summary>
  /// <param name="isActive"></param>
  void SetAciveIcon(bool isActive) {

    if (_activeScene.name != "Menu" && _activeScene.name != "Map") return;

    SpecialProductIcon iconObject = UiController.ShowUi<SpecialProductIcon>();

    iconObject.gameObject.SetActive(isActive);
    iconObject.transform.SetAsLastSibling();
  }

  /// <summary>
  /// Подтверждение покупки
  /// </summary>
  /// <param name="productName"></param>
  /// <param name="flag"></param>
  void ConfirmBye(Assets.Scripts.Billing.SpecialBillingProduct product) {
    UpdateLastShow(true);
    ShowSaleProduct(product);
  }

  /// <summary>
  /// Логирование покупки
  /// </summary>
  /// <param name="productName">Название продукта</param>
  /// <param name="flag">Успешная покупка</param>
  void LogBye(string productName, bool flag) {
    if (flag) {
      YAppMetrica.Instance.ReportEvent("Инап: покупка " + productName);
      GAnalytics.Instance.LogEvent("Инап", "Покупка", productName.ToString(), 1);
    }
  }

  [System.Serializable]
  private class SaveData {
    public string lastTime;
    public int count;
  }

}
