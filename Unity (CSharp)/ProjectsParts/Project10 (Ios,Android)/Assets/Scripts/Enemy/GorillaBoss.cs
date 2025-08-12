using UnityEngine;
using System.Collections;
using Spine.Unity;

/// <summary>
/// Босс Горилла
/// </summary>
public class GorillaBoss : EnemyBoss {

  /*
  public enum GorillaPhase {
    start,
    firstGate,
    secondGate,
    thirdGate
  }

  /// <summary>
  /// Текущая фаза гориллы
  /// </summary>
  public GorillaPhase gorillaPhase;
  */
  /// <summary>
  /// Номер появления
  /// </summary>
  int showNumber;

  /// <summary>
  /// Массив прислужников
  /// </summary>
  public GameObject[] minions;

  /// <summary>
  /// Ссылка на основную графику гориллы
  /// </summary>
  public GameObject graphicGorilla;

  /// <summary>
  /// Количество живых миньенов
  /// </summary>
  int minionsCount = 0;

  /// <summary>
  /// Простая анимация поддержки
  /// </summary>
  [SpineAnimation(dataField: "skeletonAnimation")]
  public string idleAnim = "";

  /// <summary>
  /// Время воспроизведения анимации
  /// </summary>
  float countIdleAnim;
  /// <summary>
  /// Общая анимация трона
  /// </summary>
  string idolMainAnim = "Idol_Main";
  /// <summary>
  /// Анимация машет кулаком
  /// </summary>
  string screamAnim = "Scream";

  string cryStartAnim = "Cry_Start";
  string cryIdolAnim = "Cry_Idol";
  string cryEndAnim = "Cry_End";

  string cryGroundStartAnim = "CryGround_Start";
  string cryGroundIdolAnim = "CryGround_Idol";
  string cryGroundEndAnim = "CryGround_End";

  string fistStartAnim = "Fist_Start";
  string fistIdolAnim = "Fist_Idol";
  string fistEndAnim = "Fist_End";

  string laughStartAnim = "Laugh_Start";
  string laughIdolAnim = "Laugh_Idol";
  string laughEndAnim = "Laugh_End";

  string wonderStartAnim = "Wonder_Start";
  string wonderIdolAnim = "Wonder_Idol";
  string wonderEndAnim = "Wonder_End";

  string throneTremorAnim = "Throne_Tremor";
  string throneCriticalAnim = "Throne_Critical";

  /// <summary>
  /// Анимация трона
  /// </summary>
  string throneSpineAnim;

  /// <summary>
  /// Время между атаками
  /// </summary>
  public FloatSpan timeAttack;

  /// <summary>
  /// Следующее время атаки
  /// </summary>
  float nextTimeAttack;
  /// <summary>
  /// Лжидание выполнения воздушной атаки
  /// </summary>
  bool attackReady;
  /// <summary>
  /// Активная воздушная атака
  /// </summary>
  bool activeAirAttack;

  Vector3 localvelocity = Vector3.zero;

  /// <summary>
  /// Шаг в фазе
  /// 
  /// <para>В фазе Start: 0 - бежит до</para>
  /// 
  /// </summary>
  int stepGorilla;

  /// <summary>
  /// Переопределение старта
  /// </summary>
  public override void Start() {
    base.Start();
    /*
    for (int i = 0 ; i < minions.Length ; i++) {
      if (minions[i].gameObject.activeInHierarchy) {
        minions[i].GetComponent<Enemy>().OnDead += DeadMinion;
        minions[i].GetComponent<ClassicEnemy>().SetBoss(this);
        //minions[i].transform.localScale = new Vector3(-1 , 1 , 1);
        minionsCount++;
      }
    }
    */
  }

  public override void OnEnable() {
    base.OnEnable();
    minionsCount = 0;
    //MoveFunction = null;

    for (int i = 0 ; i < minions.Length ; i++) {
      if (minions[i].gameObject.activeInHierarchy) {
        minions[i].GetComponent<Enemy>().OnDead += DeadMinion;
        minions[i].GetComponent<ClassicEnemy>().SetBoss(this);
        //minions[i].transform.localScale = new Vector3(-1 , 1 , 1);
        minionsCount++;
      }
    }

    throneSpineAnim = "";
    SetAnimation(0 , idolMainAnim , true);
  }

  void OnDestroy() {
    for (int i = 0 ; i < minions.Length ; i++) {
        minions[i].GetComponent<Enemy>().OnDead -= DeadMinion;
    }
  }
  
  /// <summary>
  /// Установка фазы
  /// </summary>
  /// <param name="phase">Номер фазы (ворот)</param>
  public void SetShowNumber(int newNumber) {

    //gorillaPhase = newPhase;
    showNumber = newNumber;
    stepGorilla = 0;

    if(showNumber > 0)
      RunnerController.Instance.SetBossPhase();

  }

  public override void OnDisable() {
    base.OnDisable();

    for (int i = 0 ; i < minions.Length ; i++) {
      minions[i].GetComponent<Enemy>().OnDead -= DeadMinion;
    }

  }

  public override void Update() {
    base.Update();

    if(showNumber == 0 && stepGorilla == 2 && transform.position.x > CameraController.displayDiff.rightDif(0.8f)) {
      stepGorilla = 3;
      AddAnimation(1 , fistStartAnim , false , 1);
      countIdleAnim = 3;
    }

    if(showNumber > 0 && stepGorilla == 1) {
      Battle();
    }

    if (transform.position.x < CameraController.displayDiff.leftDif(2f))
      HideBoss();

    /*
    if (minionsCount > 0 && transform.position.x < CameraController.displayDiff.rightDif(0.85f) && MoveFunction == null) {

      RunnerController.instance.SetBossPhase();
      MoveFunction = Battle;
      AddAnimation(1 , laughStartAnim , false , 1);
      countIdleAnim = 3;
    }
    */
  }


  /// <summary>
  /// Событие смерти миньена
  /// </summary>
  public override void DeadMinion() {
    
    minionsCount--;

    if (minionsCount > 0)
      RearyNextAttack();
    else
      attackReady = false;

    if (throneSpineAnim == null || throneSpineAnim == "")
      throneSpineAnim = throneTremorAnim;
    else if (throneSpineAnim == throneTremorAnim) {
      throneSpineAnim = throneCriticalAnim;
    }

    ResetAnimation();

    if (throneSpineAnim == throneCriticalAnim && minionsCount > 0) {
      AddAnimation(1 , wonderStartAnim , false);
      countIdleAnim = 3;
    }

    if (minionsCount == 0) {
      velocity = Vector3.zero;
      //MoveFunction = Move;
    }

    if(showNumber > 0) {
      stepGorilla++;
    }

    base.DeadMinion();
  }

  /// <summary>
  /// Обработка движения
  /// </summary>
  public override void Move() {
    base.Move();

    if (showNumber == 0)
      MovePhaseStart();

    if (showNumber > 0)
      MovePhaseAll();

    if (minionsCount == 0 && transform.localPosition.y > 0) {

      //velocity.y -= 15 * Time.deltaTime;
      localvelocity.y -= 15 * Time.deltaTime;

      if (graphicGorilla.transform.localPosition.y + localvelocity.y * Time.deltaTime <= 0) {
        graphicGorilla.transform.localPosition = new Vector3(graphicGorilla.transform.localPosition.x , 0 , graphicGorilla.transform.localPosition.z);
        //MoveFunction = null;
        AddAnimation(1 , cryGroundStartAnim , false);
        countIdleAnim = 999;
      } else
        graphicGorilla.transform.localPosition += localvelocity * Time.deltaTime;
    }

  }

  void HideBoss() {
    RunnerController.Instance.SetRunPhase();
    DeadBoss();
  }

  /// <summary>
  /// Поведение в фазе старта
  /// 
  /// <para>Необходимо подбижать к игроку, отнять кристалл и убежать</para>
  /// </summary>
  void MovePhaseStart() {
    velocity.x = RunnerController.RunSpeed;
    if(stepGorilla == 0 || stepGorilla == 2 || stepGorilla == 4) {
      velocity.x += 5;
    }
    transform.position += velocity * Time.deltaTime;
    if (transform.position.x > CameraController.displayDiff.rightDif(1.3f)) {
      HideBoss();
      gameObject.SetActive(false);
    }
  }

  void MovePhaseAll() {

    velocity.x = RunnerController.RunSpeed;

    if (stepGorilla == 0) {
      velocity.x -= 5;

      if (transform.position.x < CameraController.displayDiff.rightDif(0.85f)) {
        stepGorilla++;
        AddAnimation(1 , laughStartAnim , false , 1);
        countIdleAnim = 3;
      }

    }

    // В финальном показе не убегает, а падает на землю
    if (stepGorilla >= 2 && showNumber >= 3)
      velocity.x = 0;

    if (stepGorilla == 2 && showNumber < 3) {
      velocity.x += 5;

      if (transform.position.x > CameraController.displayDiff.rightDif(1.3f)) {
        HideBoss();
        gameObject.SetActive(false);
      }

    }
    transform.position += velocity * Time.deltaTime;
  }


  public override void AnimEnd(Spine.AnimationState state , int trackIndex) {

    base.AnimEnd(state , trackIndex);

    if (trackIndex == 1) {

      if (state.GetCurrent(trackIndex).ToString() == cryStartAnim) {
        ResetAnimation();
        SetAnimation(1 , cryIdolAnim , true);
      }

      if (state.GetCurrent(trackIndex).ToString() == cryEndAnim) {
        ResetAnimation();
      }

      if (state.GetCurrent(trackIndex).ToString() == cryGroundStartAnim) {
        ResetAnimation();
        SetAnimation(1 , cryGroundIdolAnim , true);
      }

      if (state.GetCurrent(trackIndex).ToString() == cryGroundEndAnim) {
        ResetAnimation();
      }

      if (state.GetCurrent(trackIndex).ToString() == fistStartAnim) {
        ResetAnimation();
        SetAnimation(1 , fistIdolAnim , true);
      }

      if (state.GetCurrent(trackIndex).ToString() == fistEndAnim) {
        ResetAnimation();
      }

      if (state.GetCurrent(trackIndex).ToString() == laughStartAnim) {
        ResetAnimation();
        SetAnimation(1 , laughIdolAnim , true);
      }

      if (state.GetCurrent(trackIndex).ToString() == laughEndAnim) {
        ResetAnimation();
        AddAnimation(1 , screamAnim , false);
      }

      if (state.GetCurrent(trackIndex).ToString() == screamAnim) {
        ResetAnimation();
        nextTimeAttack = Time.time + 1;
        attackReady = true;
      }

      if (state.GetCurrent(trackIndex).ToString() == wonderStartAnim) {
        ResetAnimation();
        SetAnimation(1 , wonderIdolAnim , true);
      }

      if (state.GetCurrent(trackIndex).ToString() == wonderEndAnim) {
        ResetAnimation();
      }

    }

  }

  public override void AnimComplited(Spine.AnimationState state , int trackIndex , int loopCount) {
    if (trackIndex == 1) {

      if (state.GetCurrent(trackIndex).ToString() == cryIdolAnim) {
        if (countIdleAnim == loopCount) {
          ResetAnimation();
          SetAnimation(1 , cryEndAnim , false);
        }
      }

      if (state.GetCurrent(trackIndex).ToString() == cryGroundIdolAnim) {

        if (2 == loopCount) {
          HideBoss();
        }

        if (countIdleAnim == loopCount) {
          ResetAnimation();
          SetAnimation(1 , cryGroundEndAnim , false);
        }
      }

      if (state.GetCurrent(trackIndex).ToString() == fistIdolAnim) {
        if (countIdleAnim == loopCount) {
          ResetAnimation();

          if(showNumber == 0) {
            stepGorilla++;
          }else {
            AirStoneAttack.instance.OnSpecialBarrier(new ExEvent.RunEvents.SpecialBarrier(true , SpecialBarriersTypes.airStone));
            activeAirAttack = true;
          }

          SetAnimation(1 , fistEndAnim , false);
        }
      }

      if (state.GetCurrent(trackIndex).ToString() == laughIdolAnim) {
        if (countIdleAnim == loopCount) {
          ResetAnimation();
          SetAnimation(1 , laughEndAnim , false);
        }
      }

      if (state.GetCurrent(trackIndex).ToString() == wonderIdolAnim) {
        if (countIdleAnim == loopCount) {
          ResetAnimation();
          SetAnimation(1 , wonderEndAnim , false);
        }
      }

    }
  }

  public override void ResetAnimation() {
    base.ResetAnimation();
    if (minionsCount > 0) {
      SetAnimation(idolMainAnim , true);

      if (throneSpineAnim != null && throneSpineAnim != "")
        SetAnimation(2 , throneSpineAnim , true);
    }

    if (!activeAirAttack && !attackReady)
      RearyNextAttack();
  }

  void Battle() {

    if (attackReady && nextTimeAttack <= Time.time) {
      attackReady = false;
      AddAnimation(1 , fistStartAnim , false);
      countIdleAnim = 3;
    }

  }

  void EndAirStone() {
    BoxSpawner.instance.GetTopWeaponBox();
    RearyNextAttack();
    activeAirAttack = false;
  }
	
  void RearyNextAttack() {
    nextTimeAttack = Time.time + Random.Range(timeAttack.min , timeAttack.max);
    attackReady = true;
  }

  public override void OnTriggerEnter2D(Collider2D oth) {
    base.OnTriggerEnter2D(oth);

    if(LayerMask.LayerToName(oth.gameObject.layer) == "Player") {

      // При первом пересечерии с плеером, застынем, изображая процесс отнятия кристалла
      if(showNumber == 0) {
        if(stepGorilla == 0) {
          stepGorilla = 1;
          Invoke("PahaseStartGoWishCristal" , 1);
        }
      }
      
    }

  }

  void PahaseStartGoWishCristal() {
    RunnerController.Instance.StartRun();
    StartDecor.Instance.StartBossPlay();
    RunnerController.Instance.SetBossPhase();
    stepGorilla = 2;
  }

}
