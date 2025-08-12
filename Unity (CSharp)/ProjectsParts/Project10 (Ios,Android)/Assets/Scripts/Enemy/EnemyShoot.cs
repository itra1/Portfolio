using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;

[System.Serializable]
public struct WeaponSprite {
  [SpineAtlasRegion]
  public string spineRegion;
  public GameObject weaponPref;
}

/// <summary>
/// Параметры оружия
/// </summary>
[System.Serializable]
public struct WeaponParametrs {
  public EnenyWeaponEnum type;                                      // Используемый тип оружия
  //public float timeMin;                                              // Минимальное время выстрела
  //public float timeMax;                                              // Максимальное время выстрела
  //float nextTime;                                                    // Время средующего выстрела
  [Range(0,1)]
  public float probability;
  [HideInInspector]
  public float lastDistance;
}

/// <summary>
/// Контроллер атаки Врага
/// </summary>
public class EnemyShoot : MonoBehaviour {

  GeneralEnemy enemy;                                              // Основной контроллер
  ClassicEnemy classicEnemy;
  [SerializeField]
  SkeletonAnimation skeletonAnimation;               // Анимация Скелетаs
  EnemyMove play;                                                     // Контроллер бега
  public AtlasAsset atlasAsset;

  public GameObject distAttackObject;                                 // Объект дистанционной атаки
  public WeaponSprite[] weaponSpriteParam;                            // Соотношение спрайта и оружия
  int weaponActiveNumber;                                             // Активное оружие

  public Bone folow;                                                  // Кость за которой следим
  public SkeletonRenderer skeletonRenderer;
  [SpineBone(dataField: "skeletonRenderer")]
  public string boneName;
  public Bone bone;
  private Vector3 bulletStartPosition {
    get {

      if(bone == null)
        bone = skeletonRenderer.skeleton.FindBone(boneName);

      return skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
    }
  }
  protected Transform skeletonTransform;
  [HideInInspector]
  public GameObject enemyObject;
	CapsuleCollider capsula;
	
  #region Movement
  Vector3 velocity = Vector2.zero;                                    // Рассчет смещения
  public float bodyRunSpeed;
  public float bodyJumpSpeed;
  public float bodyJumpSpeedNow;
  public float bodyAttackGravity;
  #endregion

  #region AttackBody
  [HideInInspector]
  public bool bodyAttack;                           // Флаг выполнения атаки телом
  [HideInInspector]
  public int bodyStep;                              // Шаг атаки телом
                                                    //float attackStop;                                                   // Позиция по X когда прыжок прекращается
  Vector3 lastPositionBody;                                           // Предыдущая позиция при атаки телом
                                                                      //bool bodyDownVector;                                                // Флаг падения при атаки телом
  #endregion

  #region Runs
  [HideInInspector]
  public bool runAttack;
  [HideInInspector]
  public float runAttackDistanceKoef;
  [HideInInspector]
  public bool runFullAttack;
  [HideInInspector]
  public bool runFullAttackBack;
  GameObject player;
  #endregion

  #region Animation
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string runIdleAnim = "";                                     // Анимация полета
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string jumpIdleAnim = "";                                    // Анимация полета
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string flyIdleAnim = "";                                     // Анимация падения
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string failAnim = "";                                        // Анимация падения
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string layIdleAnim = "";                                     // Анимация падения
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string distAttackAnim = "";                                  // Анимация акаки копьем
  #endregion

  void OnEnable() {
    skeletonTransform = skeletonRenderer.transform;
    enemy = GetComponent<GeneralEnemy>();
    play = GetComponent<EnemyMove>();
    classicEnemy = GetComponent<ClassicEnemy>();
    bodyAttack = false;
    runAttack = false;
    runFullAttack = false;
    runFullAttackBack = false;

    // Инифиализация аудио
    InitAudio();

  }
  void Update() {
    // Атака телом
    if(bodyAttack) BodyAttack();
    if(runAttack) RunAttack();
    if(runFullAttack) RunAttack();
    if(runFullAttackBack) RunFullAttackBack();

    //attackTransform = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));

    if(enemyObject != null) enemyObject.transform.position = bulletStartPosition + new Vector3(0, -1, 0);
  }

  void BodyAttack() {

    if(!play.isGround && bodyStep <= 1) {
      bodyAttack = false;
      bodyStep = 0;
    }

    // Бежим
    if(bodyStep == 1) {
      enemy.SetAnimation(play.runIdleAnim, true);
      velocity.x = 2f + RunnerController.RunSpeed;
      velocity.y = 0;
      transform.position += velocity * Time.deltaTime;

      //player = GameObject.Find("Player");

      float dist = Vector3.Distance(transform.position, player.transform.position);

      if(dist <= 10) {
        // Проигрываем звук
        PlayBodyAttackAudio();

        velocity.y = Random.Range(5f, 8f);
        bodyJumpSpeedNow = bodyJumpSpeed;
        if(velocity.y > 7) {
          bodyJumpSpeedNow -= (velocity.y - 7) * 1.3f;
        }
        bodyStep = 2;
				capsula.direction = 0;
				capsula.center = new Vector3(0, 0.35f, 0);
				enemy.SetGraphicLocalPosition(new Vector3(0f, 0f, 0f));
				classicEnemy.livelineInst.transform.localPosition = new Vector3(0f, 1.3f, 0f);
			}
    }

    // прыгием
    if(bodyStep == 2) {

      velocity.y -= bodyAttackGravity * Time.deltaTime;
      velocity.x = bodyJumpSpeedNow + RunnerController.RunSpeed;
      enemy.SetAnimation(flyIdleAnim, true);

      float angle = Vector3.Angle(Vector3.up, velocity);
      transform.position += velocity * Time.deltaTime;
			
      enemy.SetGraphicLocalAngle(new Vector3(0f, 0f, 90f-angle));

      Collider[] isGrounded = Physics.OverlapSphere(transform.position+new Vector3(0,0,0), play.groundRadius, play.groundMask);
      if(lastPositionBody.y > transform.position.y && isGrounded.Length > 0 && transform.position.x > CameraController.displayDiff.transform.position.x) {
				enemy.SetGraphicLocalAngle(new Vector3(0f, 0f, 0f ));

        bodyStep = 3;
        Vector2 pos = new Vector2(transform.position.x, isGrounded[0].transform.position.y);
        GetComponent<ShadowController>().SetDiff(new Vector3(0.5f, 0, 0), new Vector3(0.3f, 0, 0));
        transform.position = pos;
        enemy.AddAnimation(1, failAnim, false, 0);
				enemy.SetGraphicLocalPosition(Vector3.zero);
				// Проигрываем звук
				PlayBodyAttackAudio();
      }
    }


    if(bodyStep == 3) {
      //enemy.graphic.transform.localPosition = new Vector3(0.31f, 0.71f, 0f);
      enemy.SetAnimation(layIdleAnim, true);

      if((play.boundary.xMax + 0.2f >= transform.position.x)) {
        GetComponent<ShadowController>().SetDeff();
        //enemy.graphic.transform.localPosition = new Vector3(0f, 0f, 0f);
        enemy.SetGraphicLocalPosition(Vector3.zero);
        enemy.ResetAnimation();
				classicEnemy.livelineInst.transform.localPosition = new Vector3(0f, 2.2f, 0f);
				enemy.SetAnimation(runIdleAnim, true);
        bodyAttack = false;
        bodyStep = 0;


				capsula.direction = 1;
				capsula.center = new Vector3(0, 0.9f, 0);

				transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        if(classicEnemy != null && classicEnemy.livelineInst != null)
          classicEnemy.livelineInst.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        //enemy.graphic.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        enemy.SetGraphicLocalAngle(Vector3.zero);
      }
    }
    lastPositionBody = transform.position;
  }

  void RunAttack() {
    if(player) {
      if(transform.position.x >= CameraController.displayDiff.rightDif(runAttackDistanceKoef) || transform.position.x - 0.5f > player.transform.position.x) {
        runAttack = false;
        enemy.lastDistanceShoot = RunnerController.playerDistantion;
      }
    }
  }
  
  void RunFullAttack() {

    if(transform.position.x >= CameraController.displayDiff.rightDif(0.75f)) {
      runFullAttack = false;
      runFullAttackBack = true;
    }
  }

  void RunFullAttackBack() {
    if(transform.position.x <= CameraController.displayDiff.leftDif(0.5f)) {
      runFullAttackBack = false;
    }
  }

  public void Shoot(EnenyWeaponEnum weaponType, GameObject enemyObj = null) {

    if(play.isStoped) return;

    if(weaponType == EnenyWeaponEnum.body) {
			player = Player.Jack.PlayerController.Instance.gameObject;

      bodyAttack = true;
			capsula = GetComponent<CapsuleCollider>();
			bodyStep = 1;
    }

    if(weaponType == EnenyWeaponEnum.run) {
      runAttack = true;
      float temp = (Random.Range(0.4f, 0.8f) - 0.5f) * 2 ;

      if(temp == 0) temp = 0.01f;

      runAttackDistanceKoef = temp;

      player = Player.Jack.PlayerController.Instance.gameObject;
		}
    
    if(weaponType == EnenyWeaponEnum.runfull) {
      runFullAttack = true;
      player = Player.Jack.PlayerController.Instance.gameObject;
		}

    if(enemyObj != null) enemyObject = enemyObj;

    if(weaponType == EnenyWeaponEnum.spear
        || weaponType == EnenyWeaponEnum.head
        || weaponType == EnenyWeaponEnum.boomerang
        || weaponType == EnenyWeaponEnum.enemy) {
      enemy.AddAnimation(1, distAttackAnim, false, 0);
      StartCoroutine(ResetAnim(2));

    }

    if(weaponType == EnenyWeaponEnum.underwear) {
      enemy.AddAnimation(1, distAttackAnim, false, 0);
      StartCoroutine(ResetAnim(2));
      StartCoroutine(AddSprite(0.3f));
      weaponActiveNumber = Random.Range(0, weaponSpriteParam.Length);
      if(enemy.runnerPhase == RunnerPhase.run & Random.value <= 0.3f) enemy.PlayIdleAudio();
    }

  }

  IEnumerator AddSprite(float time) {
    yield return new WaitForSeconds(time);
    skeletonRenderer.skeleton.SetAttachment(boneName, weaponSpriteParam[weaponActiveNumber].spineRegion);
  }

  IEnumerator ResetAnim(float time) {
    yield return new WaitForSeconds(time);
    enemy.ResetAnimation();
  }

  public void SpawnWeapon(EnenyWeaponEnum weaponType) {
    if(!distAttackObject && weaponType != EnenyWeaponEnum.enemy) {
      Debug.LogError("Не указан объект для дистанционной атаки");
      return;
    }


    if(weaponType == EnenyWeaponEnum.spear) {
      // Instantiate(distAttackObject, attackTransform, Quaternion.identity);
      GameObject weapon = Pooler.GetPooledObject("EnemySpear");
      weapon.transform.position = bulletStartPosition;
      weapon.SetActive(false);
      weapon.SetActive(true);
    }

    if(weaponType == EnenyWeaponEnum.head) {
			GameObject weapon = Pooler.GetPooledObject(distAttackObject.name);
			weapon.transform.position = bulletStartPosition;
			weapon.SetActive(true);
			//Instantiate(distAttackObject, attackTransform, Quaternion.identity);
    }

    if(weaponType == EnenyWeaponEnum.boomerang) {
			GameObject weapon = Pooler.GetPooledObject(distAttackObject.name);
			weapon.transform.position = bulletStartPosition - new Vector3(0, 1f, 0);
			weapon.SetActive(true);
			//Instantiate(distAttackObject, attackTransform - new Vector3(0, 1f, 0), Quaternion.identity);
    }

    if(weaponType == EnenyWeaponEnum.pillow) {
      GameObject obj = Instantiate(distAttackObject , bulletStartPosition - new Vector3(0 , 1f , 0) , Quaternion.identity) as GameObject;
      obj.GetComponent<BoomerangController>().targets = gameObject;
    }

    if(weaponType == EnenyWeaponEnum.enemy) {
      if(enemyObject != null && enemyObject.activeInHierarchy) {
        enemyObject.GetComponent<AztecEnemy>().Toss();
        enemyObject = null;
      }
    }

    if(weaponType == EnenyWeaponEnum.underwear) {
      Instantiate(weaponSpriteParam[weaponActiveNumber].weaponPref, bulletStartPosition, Quaternion.identity);
    }

    //StartCoroutine(ResetAnim(0.2f));
  }
  #region audio

  [Header("Audio")]

  AudioSource audioComp;                  // Компонент аудио
  public AudioClip[] bidyAttackAudio ;        // Звуки при беге

  void InitAudio() {
    audioComp = GetComponent<AudioSource>();
  }

  public void PlayBodyAttackAudio() {

    if(bidyAttackAudio.Length > 0) {
      audioComp.PlayOneShot(bidyAttackAudio[Random.Range(0, bidyAttackAudio.Length)], 1);
    }
  }

  #endregion


}
