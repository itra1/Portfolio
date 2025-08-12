using UnityEngine;
using System.Collections;

/// <summary>
/// Структура урона наносимое оружием
/// </summary>
[System.Serializable]
public struct BulletJsonParabetrs {
  public int posX;                  // Позиция по координате X
  public int posY;                  // Позиция по координате Y
  public string dir;                // Вектор направления персонажа (визуализация поворота, тоесть не нужно)
}

/// <summary>
/// Контроллер патрона
/// </summary>
public class Bullet : MonoBehaviour {

  public GameObject graphicObject;                  // Ссылка на компонент графики
  protected Rigidbody2D rb;                         // Компонент твердого тела
  [HideInInspector]
  public float speed;                               // Скорость полета пули
  [HideInInspector]
  public Player playerOwner;                        // Плеер, совершивший выстрел
  [HideInInspector]
  public float rangeAccuracy;                       // Разброс выстрела
  protected Vector3 shootVector = Vector3.left;     // Вектор полета снаряда
  public Vector3 targetPoint;
  public TrailRenderer trail;
  protected MoveObjecsElem moveParam = new MoveObjecsElem();

  protected int sceneId;

  public virtual void Start() { }

  public virtual void OnEnable() {
    rb = GetComponent<Rigidbody2D>();
    if(trail != null) trail.Clear();
    MoveObjects.OnMove += OnMove;
    ObjectDestroyed.OnDestroyObject += OnDestroyObject;
  }

  public virtual void OnDisable() {
    ObjectDestroyed.OnDestroyObject -= OnDestroyObject;
    MoveObjects.OnMove -= OnMove;
  }

  protected virtual void Update() {
    Move();
  }

  /// <summary>
  /// Установка идентификатора
  /// </summary>
  /// <param name="id"></param>
  public void SetId(int id) {
    this.sceneId = id;
  }

  public virtual void OnMove(MoveObjecsElem newMove) {
    if(newMove.id != sceneId) return;
    moveParam = newMove;
  }

  /// <summary>
  /// Обработка движения
  /// </summary>
  protected virtual void Move() {
    transform.position += (Vector3)moveParam.velocity * Time.deltaTime;
  }

  public virtual void Init(MoveObjecsElem moveParam) { }

  /// <summary>
  /// Деактивация элемента
  /// </summary>
  protected virtual void Deactive() {
    DeactiveSFX();
    gameObject.SetActive(false);
  }

  protected virtual void DeactiveSFX() { }

  void OnDestroyObject(int id) {
    if(this.sceneId != id) return;

    Deactive();
  }

}
