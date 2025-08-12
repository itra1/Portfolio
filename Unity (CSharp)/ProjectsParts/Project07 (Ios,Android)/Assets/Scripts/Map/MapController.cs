using UnityEngine;

namespace Game.Map {

  public class MapController: Singleton<MapController> {

    private World _world;

    private void OnEnable() {
      Initiate();
    }

    private void Initiate() {
      OpenMapUi();
      GameManager.Instance.PlayBackGroundSound("BackGround/menuBack");
      LoadWorld();
    }

    private void LoadWorld() {
      if (_world != null) {
        if (_world.Block == UserManager.Instance.SelectBlock)
          return;
        else
          Destroy(_world.gameObject);
      }

      World world = Resources.Load<World>("Worlds/" + UserManager.Instance.SelectBlock);
      GameObject inst = Instantiate(world.gameObject, transform);
      _world = inst.GetComponent<World>();
    }

    public void OpenMapUi() {
      MapUi panel = UiController.GetUi<MapUi>();
      panel.gameObject.SetActive(true);
      panel.onSetting = OpenSettingDialog;
      panel.onBase = () => { GameManager.Instance.LoadScene("Base"); };
      panel.onBackLocation = () => {
        UserManager.Instance.SelectBlock--;
        GameManager.Instance.LoadScene("Map");
      };
      panel.onNextLocation = () => {
        UserManager.Instance.SelectBlock++;
        GameManager.Instance.LoadScene("Map");
      };
    }

    private void OpenSettingDialog() {
      SettingDialog panel = UiController.GetUi<SettingDialog>();
      panel.Show();
    }

    public void ClickMapPoint(Location location) {

      MapDialog panel = UiController.GetUi<MapDialog>();
      panel.SetData(location);
      panel.Show();
      panel.OnNext = () => {
        GameManager.Instance.StartBattle(location); };
      panel.OnBack = panel.CloseButton;

    }
    
  }
}