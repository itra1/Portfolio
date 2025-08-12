using Spine.Unity;
using UnityEngine;

public class ChocolateWeapon: MonoBehaviour {

  SkeletonRenderer skeletonRenderer;
  public GameObject[] bolts;

  public float speedX;                                // Скорость
  float angle;                                        // Угол поворота
  Vector3 velocity = Vector3.zero;                    // Рассчет смещения
  public float gravity;                               // Значение графитации

  public LayerMask groundMask;                        // Слой с поверхностью
  public float groundRadius;                          // Радиус определения поверхности
  [HideInInspector]
  public bool act;                                    // Флаг активности, что может наносить повреждение

  public WeaponTypes thisWeaponType;                  // Текущий тип оружия
  public GameObject graph;                            // Ссылка на графиескую составляющую, используется для поворота картинки


  public GameObject sfxEffect;

  float rotationValue;

  bool shoot = false;

  void Start() {
    skeletonRenderer = Player.Jack.PlayerController.Instance.animation.skeletonRenderer;
    GetComponent<BoneFollower>().skeletonRenderer = skeletonRenderer;

    skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event += AnimEvent;
    act = true;

    rotationValue = 1000;

  }

  void Update() {

    if (shoot) {

      // Гравитация
      velocity.y -= gravity * Time.deltaTime;

      Collider[] isGrounded = Physics.OverlapSphere(transform.position, groundRadius, groundMask);
      if (isGrounded.Length > 0 & velocity.y < 0)
        velocity.y = 0;

      // Вращение
      angle += rotationValue * Time.deltaTime;

      if ((graph.transform.eulerAngles.z < 115 && angle >= 115) && isGrounded.Length > 0)
        angle = 115;

      if ((graph.transform.eulerAngles.z < 260 && angle >= 260) && isGrounded.Length > 0)
        angle = 260;

      graph.transform.eulerAngles = new Vector3(0, 0, angle);

      // Скорость
      velocity.x = RunnerController.Instance.runSpeedActual;
      velocity.x += speedX;

      if ((graph.transform.eulerAngles.z == 115 || graph.transform.eulerAngles.z == 260) && isGrounded.Length > 0) {
        velocity.x = 0;
        rotationValue = 0;
      }

      transform.position += velocity * Time.deltaTime;

      if (velocity.y < 0) ActiveteConf();
    }

    //Уничтожение, при улетании объекта
    if (transform.position.x <= 0 || transform.position.y <= 0) {
      Destroy(gameObject);
    }
  }

  void OnDestroy() {
    skeletonRenderer.GetComponent<SkeletonAnimation>().state.Event -= AnimEvent;
  }

  void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) {
    GetComponent<BoneFollower>().enabled = false;

    transform.eulerAngles = new Vector3(0, 0, 0);
    angle = 180;
    graph.transform.eulerAngles = new Vector3(0, 0, angle);
    velocity.y = 15f;
    shoot = true;
  }

  void ActiveteConf() {
    foreach (GameObject bolt in bolts) {
      Instantiate(bolt, transform.position, Quaternion.identity);
    }
    GameObject eff = Instantiate(sfxEffect, transform.position, Quaternion.identity) as GameObject;
    Destroy(eff, 10);
    Destroy(gameObject);
  }
}
