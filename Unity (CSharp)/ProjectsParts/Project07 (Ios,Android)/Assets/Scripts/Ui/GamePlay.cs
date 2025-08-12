using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlay: UiPanel {

  public System.Action onSetting;
  public Transform iconHardCash;
  public Transform iconSoftCash;
  public Transform iconEnergy;
  public Text waveText;
  public Text patronText;

  public RectTransform weaponParent;

  [SerializeField]
  private GameObject addHardCacheButton;
  [SerializeField]
  private GameObject addSoftCacheButton;
  [SerializeField]
  private GameObject addEnergyCacheButton;


  private string activeStateString {
    get {
      return (BattleController.Instance.activeState + 1).ToString() + "/" + UserManager.Instance.ActiveLocation.stageCount.ToString();
    }
  }

  protected override void OnEnable() {
    Debug.Log("OnEnable");
    base.OnEnable();
    waveText.text = activeStateString;

    addHardCacheButton.SetActive(SceneManager.GetActiveScene().name != "Game");
    addSoftCacheButton.SetActive(SceneManager.GetActiveScene().name != "Game");
    addEnergyCacheButton.SetActive(SceneManager.GetActiveScene().name != "Game");

    SetActiveWeapon(WeaponsSpawner.Instance.GetActiveWeapon());
    SetReadyWeapons(WeaponsSpawner.Instance.instanceList);
    OnWeaponChange(WeaponsSpawner.Instance.GetActiveWeapon());
  }


  public void SettingButton() {
    UiController.Instance.ClickSound();
    if (onSetting != null) onSetting();
  }


  public Animation DamagePanelAnim;
  public RectTransform lineLive;

  [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.UserDamage))]
  public void UserDamage(ExEvent.UserEvents.UserDamage eventData) {

    lineLive.sizeDelta = new Vector2((eventData.health / eventData.maxHealth) * 228.52f, lineLive.sizeDelta.y);

    DamagePanelAnim.Play("show");
  }


  [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.UserMedicineBonus))]
  public void UserMedicineBonus(ExEvent.UserEvents.UserMedicineBonus eventData) {

    lineLive.sizeDelta = new Vector2((eventData.health / eventData.maxHealth) * 228.52f, lineLive.sizeDelta.y);


  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleStart))]
  private void BattleStart(ExEvent.BattleEvents.BattleStart eventData) {
    lineLive.sizeDelta = new Vector2(228.52f, lineLive.sizeDelta.y);
    waveText.text = activeStateString;

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.UserDied))]
  public void UserDied(ExEvent.UserEvents.UserDied eventData) {
    lineLive.sizeDelta = new Vector2(0, lineLive.sizeDelta.y);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.NextWave))]
  private void NextWave(ExEvent.BattleEvents.NextWave eventData) {

    if (eventData.isBoss == true) waveText.text = "Boss";
    else waveText.text = activeStateString;
  }

  public void BaseButton() {
    UiController.Instance.ClickSound();
    ConfirmDialog dialog = UiController.GetUi<ConfirmDialog>();
    dialog.gameObject.SetActive(true);

    GameTime.timeScale = 0;
    
    dialog.SetData("confirmDialog.exitToPlane", () => {

      GameTime.timeScale = 1;
      GameManager.Instance.LoadScene("Base");
      //UnityEngine.SceneManagement.SceneManager.LoadScene("Base", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }, () => {
      GameTime.timeScale = 1;
    });


  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponChange))]
  private void WeaponChange(ExEvent.BattleEvents.WeaponChange eventData) {
    OnWeaponChange(eventData.weapon);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.WeaponShoot))]
  private void WeaponShoot(ExEvent.BattleEvents.WeaponShoot eventData) {
    OnWeaponChange(eventData.weapon);
  }

  private void OnWeaponChange(WeaponBehaviour weapon) {

    if (!weapon.IsSelected) return;

    SetActiveWeapon(weapon);

    if (weapon.unlimBullets) {
      patronText.text = "unlim";
    } else {
      patronText.text = weapon.BulletCount.ToString();
    }

  }

  private WeaponBehaviour _activeWeapon;

  private void SetActiveWeapon(WeaponBehaviour activeWeapon) {

    if (activeWeapon == null) return;

    for (int i = 0; i < weaponParent.childCount; i++)
      weaponParent.GetChild(i).gameObject.SetActive(false);

    _activeWeapon = activeWeapon;

    if (_activeWeapon.UiGuiInstance == null) {
      GameObject guiInst = Instantiate(_activeWeapon.uiGui.gameObject);
      _activeWeapon.UiGuiInstance = guiInst.GetComponent<WeaponUi>();
      RectTransform rect = guiInst.GetComponent<RectTransform>();
      guiInst.transform.SetParent(weaponParent);
      guiInst.transform.localScale = Vector2.one;
      rect.pivot = new Vector2(0.5f, 0);
      guiInst.transform.localPosition = Vector3.zero;
      rect.sizeDelta = weaponParent.sizeDelta;
    }

    _activeWeapon.UiGuiInstance.gameObject.SetActive(true);
    //OnWeaponChange(_activeWeapon);

    iconsList.ForEach(icon => icon.SetActiveWeapon(activeWeapon));

  }

  public WeaponGamePlayIcon templateIcon;
  public RectTransform weaponIconsParent;
  public List<WeaponGamePlayIcon> iconsList = new List<WeaponGamePlayIcon>();

  [ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.ReadyWeapon))]
  public void ReadyWeapon(ExEvent.BattleEvents.ReadyWeapon eventData) {
    SetReadyWeapons(eventData.weapons);
  }

  private void SetReadyWeapons(List<WeaponBehaviour> readyWeapons) {

    if (readyWeapons.Count <= 0) return;
    iconsList.ForEach(elem => Destroy(elem.gameObject));
    iconsList.Clear();

    weaponIconsParent.sizeDelta = new Vector2(readyWeapons.Count * templateIcon.GetComponent<RectTransform>().sizeDelta.x, weaponIconsParent.sizeDelta.y);

    readyWeapons.ForEach(wep => {

      GameObject inst = Instantiate(templateIcon.gameObject);
      RectTransform rectInst = inst.GetComponent<RectTransform>();

      rectInst.SetParent(templateIcon.transform.parent);
      rectInst.localScale = Vector2.one;
      rectInst.gameObject.SetActive(true);
      inst.GetComponent<WeaponGamePlayIcon>().SetWeapon(wep);
      rectInst.SetAsLastSibling();
      iconsList.Add(inst.GetComponent<WeaponGamePlayIcon>());
    });

  }

  public void ForceComplete() {
    BattleController.Instance.ForceComplete();
  }

}
