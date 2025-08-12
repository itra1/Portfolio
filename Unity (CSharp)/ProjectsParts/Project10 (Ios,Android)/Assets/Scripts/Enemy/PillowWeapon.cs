using UnityEngine;

public class PillowWeapon: MonoBehaviour {

  public float playerDamageValue;                         // Повреждение игрока
  bool playerDamage;                                      // Флаг разрешающий наносить повреждение игроку

  [SerializeField]
  private LayerMask groundMask;          // Слой с поверхностью
  [SerializeField]
  private readonly float groundRadius;            // При включеном прилипании к чему цепляться
  bool groundContact;                                     // Флаг контакта с землей
  Rigidbody thisRigidbody;
  float angle;                                            // Угол относительно вертикального вектора в полете

  public Transform groundPoint;                           // Точка определения соприкосновения с зеплей
  public AudioClip[] groundClip;                          // Звуки соприкосновения с зеплей

  [HideInInspector]
  public bool disableAudio;             // Флаг отключить звук

  bool mirrow;                                            // Отражение
  public ParticleType particle;

  void OnEnable() {
    thisRigidbody = GetComponent<Rigidbody>();
    playerDamage = true;
    thisShoot();
  }

  void FixedUpdate() {
    // Определям поверхность соприкосновения
    Collider[] grounded = Physics.OverlapSphere(groundPoint.position, groundRadius, groundMask);
    bool isGround = grounded.Length > 0 ? true : false;

    if (isGround & !groundContact) {
      if (!disableAudio && groundClip.Length > 0) {
        AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect);
      }

      if (particle != ParticleType.none)
        AllParticles.Generate(particle, groundPoint.position, 20);

      thisRigidbody.isKinematic = true;
      thisRigidbody.useGravity = false;
      groundContact = true;
      playerDamage = false;
    }

    if (!isGround) {
      if (mirrow) {
        angle += 10;
        transform.localEulerAngles = new Vector3(0f, 0f, angle);
      } else {
        angle -= 10;
        transform.localEulerAngles = new Vector3(0f, 0f, angle);
      }
    }

  }

  void thisShoot() {
    GameObject player = GameObject.Find("Player");

    float distance = 10f;
    if (player) distance = Vector3.Distance(transform.position, player.transform.position);

    Vector3 ShootVector = new Vector3(1, 1, 0);
    distance += RunnerController.RunSpeed * 0.8f;
    thisRigidbody.AddRelativeForce((ShootVector * Mathf.Sqrt((distance * 32f) / Mathf.Sin(2 * (45 * Mathf.Deg2Rad))))/** koef*/, ForceMode.Impulse);
  }

  public void Mirrow() {
    mirrow = true;
    thisRigidbody.velocity = Vector3.zero;
    transform.localEulerAngles = Vector3.zero;
    Vector3 ShootVector = new Vector3(-1, 1, 0);
    thisRigidbody.AddRelativeForce(ShootVector * 8, ForceMode.Impulse);
  }

  void OnTriggerEnter2D(Collider2D other) {

    // Срабатывание при контакте с игроком
    if (LayerMask.LayerToName(other.gameObject.layer) == "Player" && playerDamageValue > 0 && playerDamage) {
      playerDamage = false;
      if (Player.Jack.PlayerController.Instance.spearDefenderTime >= Time.time) {
        Mirrow();
      } else
        Player.Jack.PlayerController.Instance.ThisDamage(WeaponTypes.pillow, Player.Jack.DamagType.power, playerDamageValue, transform.position);
    }
  }
}
