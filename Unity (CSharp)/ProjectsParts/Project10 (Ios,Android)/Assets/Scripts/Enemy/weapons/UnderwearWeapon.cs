/*
  Контроллер оружия нижнего белья
*/

using UnityEngine;
using System.Collections;

public class UnderwearWeapon : MonoBehaviour {

	public float playerDamageValue;                         // Повреждение игрока
    bool playerDamage;                                      // Флаг разрешающий наносить повреждение игроку
    
    public Vector3 HingeJointAncor;

    [SerializeField] private LayerMask groundMask;          // Слой с поверхностью
    [SerializeField] private float groundRadius;            // При включеном прилипании к чему цепляться
    bool groundContact;                                     // Флаг контакта с землей
    Rigidbody thisRigidbody;
    float angle;                                            // Угол относительно вертикального вектора в полете

    public Transform groundPoint;                           // Точка определения соприкосновения с зеплей
    public AudioClip[] groundClip;                          // Звуки соприкосновения с зеплей
    
    [HideInInspector] public bool disableAudio;             // Флаг отключить звук

    bool mirrow;                                            // Отражение
    public ParticleType particle;
    bool HingeJointActive;

    public float jointTime;
    float thisJointTime;

    void OnEnable() {
        thisRigidbody = GetComponent<Rigidbody>();
        playerDamage = true;
        HingeJointActive = false;
        thisShoot();
    }

    void FixedUpdate() {
        // Определям поверхность соприкосновения
        Collider[] grounded = Physics.OverlapSphere(groundPoint.position, groundRadius, groundMask);
        bool isGround = grounded.Length > 0 ? true : false;

        if(isGround & !groundContact && !HingeJointActive) {
            if(!disableAudio && groundClip.Length > 0) {
                AudioManager.PlayEffect(groundClip[Random.Range(0, groundClip.Length)], AudioMixerTypes.runnerEffect, 1);
            }

            if(particle != ParticleType.none)
                AllParticles.Generate(particle, groundPoint.position, 20);

            thisRigidbody.isKinematic = true;
            thisRigidbody.useGravity = false;
            groundContact = true;
            playerDamage = false;
        }

        if(!isGround && !HingeJointActive) {
            if(mirrow) {
                angle = Vector3.Angle(Vector3.up, thisRigidbody.velocity);
                transform.localEulerAngles = new Vector3(0f, 0f, angle);
            } else {
                angle = Vector3.Angle(Vector3.up, thisRigidbody.velocity);
                transform.localEulerAngles = new Vector3(0f, 0f, -angle);
            }
        }

        if(thisJointTime > 0 && thisJointTime < Time.time) {

            thisRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            HingeJointActive = false;
            Destroy(GetComponent<HingeJoint>());
        }

    }

    void thisShoot() {
        GameObject player = GameObject.Find("Player");

        float distance = 10f;
        if(player) distance = Vector3.Distance(transform.position, player.transform.position);
        
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

    void OnTriggerEnter(Collider other) {

        // Срабатывание при контакте с игроком
        if(LayerMask.LayerToName(other.gameObject.layer) == "Player" && playerDamageValue > 0 && playerDamage) {
            playerDamage = false;
            if(other.GetComponent<Player.Jack.PlayerController>().spearDefenderTime >= Time.time) {
                Mirrow();
            } else
                other.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.underwear, Player.Jack.DamagType.power, playerDamageValue, transform.position);

            HingeJoint joint = gameObject.AddComponent<HingeJoint>();
            joint.connectedBody = other.GetComponent<Player.Jack.PlayerController>().HingeJointBodyes[Random.Range(0, other.GetComponent<Player.Jack.PlayerController>().HingeJointBodyes.Length)];
            joint.enablePreprocessing = false;
            joint.enableCollision = false;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = HingeJointAncor;
            joint.connectedAnchor = Vector3.zero;
            joint.axis = new Vector3(0, 0, 1);
            joint.useLimits = true;

            JointLimits lim = new JointLimits();
            lim.max = 270f;
            lim.min = 90f;

            joint.limits = lim;
            HingeJointActive = true;
            thisRigidbody.constraints = RigidbodyConstraints.None;
            thisJointTime = Time.time + jointTime;
        }
    }
}
