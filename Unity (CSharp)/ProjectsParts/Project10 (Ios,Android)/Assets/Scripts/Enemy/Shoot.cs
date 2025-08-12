using UnityEngine;
using System.Collections;
/*
 * Класс снаряда 
 */
public class Shoot : MonoBehaviour {

  public bool TurnOnFlight; // Поворачивать по полету
  public bool SnapToPlatform; // Прилипание к платформам, требует дочернего объекта SnapPoint с 2D коллайдером
  public float rotateOnFly; // Врашение в полете
  public bool snapToDegree; // Застыватьпод указанымуглом
  public float snapToAngle; // Угол в которм разрешено застыть
  private Vector3 rotateAngle;

  [SerializeField]
  private int SnapColusionCount; // Количество соприкосновений с поверхностью до момента залипания
  [SerializeField]
  private LayerMask groundMask; // При включеном прилипании к чему цепляться

  public float groundSnapPoint = 0f;   // Радиус для определения соприкосновения с поверхностью
  public bool hingedFire; // Навечной огонь
  private GameObject player;   // Используется для определения набравления броска

  private Rigidbody thisRigidbody;
  public Transform snapPoint;
  private Vector3 ShootVector;    // Вектор определения броска

  float dif; // Смещение по x относительно центра платформы
  float angle;    // Угол относительно вертикального вектора в полете

  public float startAngle;

  public float soundValue;
  public AudioClip[] groundClip;
  bool groundContact;

  public bool disableAudio;

  public ParticleType particle;

  bool mirrow;

  void Start() {

    gameObject.GetComponent<Damager>().enabled = true;
    thisRigidbody = GetComponent<Rigidbody>();
    thisRigidbody.isKinematic = false;
    thisRigidbody.velocity = Vector2.zero;
    transform.localEulerAngles = Vector3.zero;
    thisShoot();

    transform.localEulerAngles = new Vector3(0f, 0f, startAngle);
  }


  void Update() {
    // Определям поверхность соприкосновения
    Collider[] grounded = Physics.OverlapSphere(snapPoint.position, groundSnapPoint, groundMask);
    bool isGround = grounded.Length > 0 ? true : false;

    if(isGround & !groundContact) {
      if(!disableAudio && groundClip.Length > 0) {
        AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect, (soundValue > 0) ? soundValue : 1);
      }

      if(particle != ParticleType.none)
        AllParticles.Generate(particle, snapPoint.position, 20);

      thisRigidbody.isKinematic = true;
      thisRigidbody.useGravity = false;
      this.GetComponent<Damager>().enabled = false;
      groundContact = true;
    }

    //Если включен поворот по вектору полета и предмет не презеплился, плавно поворачиваем
    if(TurnOnFlight && !isGround) {
      if(mirrow) {
        angle = Vector3.Angle(Vector3.up, thisRigidbody.velocity);
        transform.localEulerAngles = new Vector3(0f, 0f, angle);
      } else {
        angle = Vector3.Angle(Vector3.up, thisRigidbody.velocity);
        transform.localEulerAngles = new Vector3(0f, 0f, -angle);
      }
    } else if(rotateOnFly != 0 && !TurnOnFlight) {
      rotateAngle = new Vector3(0f, 0f, transform.localEulerAngles.z + rotateOnFly * Time.deltaTime);

      if(!isGround)
        transform.localEulerAngles = rotateAngle;
      else
        transform.localEulerAngles = new Vector3(0f, 0f, snapToAngle);
    }
  }

  void thisShoot() {
    player = Player.Jack.PlayerController.Instance.gameObject;

    // Опеределяем вектор полета
    if(hingedFire) // Если навесной
    {
      ShootVector = new Vector3(Random.Range(0.3f, 1.2f), Random.Range(1.9f, 2.4f), 0);
      float mnoj = 8f;
      thisRigidbody.AddRelativeForce(ShootVector * mnoj, ForceMode.Impulse);
    } else // Если обычный
      {
      if(player) {
        float distance = Vector3.Distance(transform.position, player.transform.position)/*+Random.Range(-0.5f,0.5f)*/;
        Vector3 ShootVector = new Vector3(1, 1, 0);
        distance += RunnerController.RunSpeed * 0.9f;

        thisRigidbody.AddRelativeForce((ShootVector * Mathf.Sqrt((distance * 32f) / Mathf.Sin(2 * (45 * Mathf.Deg2Rad))))/** koef*/, ForceMode.Impulse);
      }
    }
  }

  public void Mirrow() {
    mirrow = true;
    thisRigidbody.velocity = Vector3.zero;
    transform.localEulerAngles = Vector3.zero;
    Vector3 ShootVector = new Vector3(-1, 1, 0);
    thisRigidbody.AddRelativeForce(ShootVector * 8, ForceMode.Impulse);
  }

}
