using UnityEngine;

/// <summary>
/// Препядствие на сцене
/// </summary>
public class Barrier : MonoBehaviour {
  
  public GameObject graphic;          // Графика
  public GameObject livePanel;        // Панель жизней
  public Transform liveValue;         // Значение жизней

  public bool isDestroyed;
  PolygonCollider2D polygoneCollider;  
  int activePlayer;
  int sceneId;
    
  void Awake() { }
  void Start() {
    livePanel.SetActive(false);
    liveValue.localScale = Vector2.one;
    MapManager.instance.AddBarrier(GetComponent<Collider2D>());
  }
  void OnEnable() {
    if(MapManager.instance != null)
      OnRotateScene(MapManager.instance.leftPlayer);
    HealthUpdate.OnHealth += OnHealth;
    ObjectDestroyed.OnDestroyObject += OnDestroyObject;
    MapManager.OnRotateScene += OnRotateScene;
  }
  void OnDisable() {
    MapManager.OnRotateScene -= OnRotateScene;
    HealthUpdate.OnHealth -= OnHealth;
    ObjectDestroyed.OnDestroyObject -= OnDestroyObject;
  }

  void OnDestroy() { }

  public void Init(int id) {
    sceneId = id;
  }

  void OnDestroyObject(int sceneId) {
    if(this.sceneId == sceneId) Destroy(gameObject);
  }

  void OnHealth(HealthUpdate.HealthData healthData) {
    if(sceneId != healthData.sceneId) return;
    livePanel.SetActive(true);
    SetHealthValue(healthData.healthMax, healthData.currentHealth);
  }

  public void SetHealthValue(float healtMax, float healthCurrent) {
    if(healtMax != healthCurrent) livePanel.SetActive(true);
    liveValue.localScale = new Vector2(healthCurrent / healtMax, 1);
  }
  
  void OnRotateScene(int newActivePlayer) {

    if(newActivePlayer != activePlayer)
      transform.position = MapManager.instance.PositionMirrow(transform.position);
    activePlayer = newActivePlayer;
    graphic.transform.localScale = new Vector3((activePlayer == 1 ? -1 : 1), 1, 0);
  }
  
  
}
