using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using ExEvent;

/// <summary>
/// Параметры рулетки
/// </summary>
[System.Serializable]
public struct RuletteValue {
  public float roundDegree;
  public float startRound;
  public float endRound;
  public BoostType boost;
  public int count;
  public GameObject bigRound;
  public GameObject round;
}

/// <summary>
/// Вероятность рулетки
/// </summary>
[System.Serializable]
public struct rouletteProbability {
  public BoostType boost;
  public float probability;
}

/// <summary>
/// Параметры по уровням
/// </summary>
[System.Serializable]
public struct RouletteGenerateParamerts {
  public rouletteProbability[] chance;
}

/// <summary>
/// Рулетка
/// </summary>
public class RouletteController: EventBehaviour {

  public Action<BoostType> OnUse;

  public Transform center;
  public GameObject arrow;                        // Объект стрела
  public GameObject board;                        // Объект стрела
  public RuletteValue[] rulletteType;             // Варианты значений рулетки
  public GameObject RotatePanel;                  // Объект кнопки кручения
  public RunnerController runner;                 // сцена
  public GameObject targetPoint;                  // Точка генерации шарика
  public StartDecor startObject;

  public RouletteGenerateParamerts[] rouletteParametrs;             // Параметры рулетки

  enum RoulettePhase { ready, stop, calc, end }
  RoulettePhase roulettePhase;

  private int sector;                                     // Выпавший сектор
  private int lounch;                                     // Купленный бонус
  private float angle;                                    // Угол поворота
  private float angleDiff;
  private float speed;                                    // Скорость смещения
  private float startDegree;

  public AudioClip clickClip;
  public AudioClip roulettStart;
  public AudioClip roulettIdle;
  public AudioClip roulettStop;

  float timeRotate;

  AudioSource audioSourceComponent;

  ClothesBonus rouletteClothes;

  void OnEnable() {
    audioSourceComponent = GetComponent<AudioSource>();

    UpdateValue();
    board.transform.eulerAngles = new Vector3(0, 0, 0);
    arrow.transform.eulerAngles = new Vector3(0, 0, 0);
    CalcRoulett();
    roulettePhase = RoulettePhase.ready;
  }

  /// <summary>
  /// Обновление значений рулетки при 
  /// </summary>
  void UpdateValue() {
    CalcRoulett();
  }

  /// <summary>
  /// Первоначальная расстановка параметров на рулетки
  /// </summary>
  void CalcRoulett() {

    rouletteClothes = Config.GetActiveCloth(ClothesSets.roulette);
    int rouletteLevel = PlayerPrefs.GetInt("roulette");

    for (int i = 0; i < rulletteType.Length; i++) {
      rulletteType[i].bigRound.SetActive(false);
      rulletteType[i].round.SetActive(false);
      rulletteType[i].startRound = 0;
      rulletteType[i].roundDegree = 0;
      rulletteType[i].endRound = 0;
    }

    float allDegree = 0;

    float summ = 0;
    for (int i = 0; i < rulletteType.Length; i++) {

      foreach (rouletteProbability param in rouletteParametrs[rouletteLevel].chance) {
        if (rulletteType[i].boost == param.boost) {
          rulletteType[i].roundDegree = 360 / 100 * (param.probability + (rouletteClothes.head ? param.probability * 0.25f : 0) + (rouletteClothes.spine ? param.probability * 0.25f : 0) + (rouletteClothes.accessory ? param.probability * 0.25f : 0));
          summ += rulletteType[i].roundDegree;
        }
      }

      allDegree += rulletteType[i].roundDegree;
    }

    summ -= 360;

    if (summ > 0) {
      allDegree = 0;
      float minus = 36;

      foreach (rouletteProbability param in rouletteParametrs[rouletteLevel].chance) {
        for (int i = 0; i < rulletteType.Length; i++) {

          if (rulletteType[i].boost == param.boost) {
            if (summ > 0) {
              Debug.Log(param.boost);
              rulletteType[i].roundDegree -= minus;
              summ -= minus;
              minus = summ - minus <= 0 ? summ : minus;
            }
          }
          allDegree += rulletteType[i].roundDegree;
        }
      }
    }

    startDegree = 360 - (allDegree / 2);

    float thisDegree = startDegree;
    for (int i = 0; i < rulletteType.Length; i++) {
      if (rulletteType[i].roundDegree > 0) {
        rulletteType[i].startRound = thisDegree;
        rulletteType[i].bigRound.transform.localEulerAngles = new Vector3(0, 0, 360 - thisDegree);
        rulletteType[i].bigRound.GetComponent<Image>().fillAmount = rulletteType[i].roundDegree / 360;
        rulletteType[i].bigRound.SetActive(true);

        if (i > 0) rulletteType[i - 1].bigRound.GetComponent<Image>().fillAmount += 0.001f; // Что бы убрать полоску между элементами

        rulletteType[i].endRound = thisDegree + rulletteType[i].roundDegree/* - 0.5f*/;

        rulletteType[i].startRound = (360 < rulletteType[i].startRound ? rulletteType[i].startRound + 360 : rulletteType[i].startRound);
        rulletteType[i].endRound = (360 < rulletteType[i].endRound ? rulletteType[i].endRound - 360 : rulletteType[i].endRound);

        rulletteType[i].round.SetActive(true);
        float tmpangle = 360 - (rulletteType[i].startRound + ((rulletteType[i].endRound - rulletteType[i].startRound) / 2));
        if (rulletteType[i].endRound < rulletteType[i].startRound) {
          float v1 = (360 - rulletteType[i].startRound + rulletteType[i].endRound) / 2;
          float v2 = rulletteType[i].startRound + v1;
          float v3 = 0;
          if (v2 > 360) v3 = v2 - 360;
          else v3 = v2;

          tmpangle = 360 - Mathf.Abs(v3);
        }

        arrow.transform.eulerAngles = new Vector3(0, 0, tmpangle);

        rulletteType[i].round.gameObject.transform.position = targetPoint.transform.position;
        rulletteType[i].round.gameObject.transform.eulerAngles = new Vector3(0, 0, -180);
        thisDegree = rulletteType[i].endRound;
      }
    }
    board.transform.eulerAngles = new Vector3(0, 0, 180);
  }

  /// <summary>
  /// Случайные значения рулетки
  /// <para>Используется для отладки</para>
  /// </summary>
  public void randomValues() {
    rulletteType[0].count = Random.Range(0, 3);
    rulletteType[1].count = Random.Range(0, 3);
    rulletteType[2].count = Random.Range(0, 3);
    rulletteType[3].count = Random.Range(0, 3);
    rulletteType[4].count = Random.Range(0, 3);

    CalcRoulett();
  }

  void Update() {
    Rotation();
  }

  /// <summary>
  /// Вращение
  /// </summary>
  void Rotation() {

    if (roulettePhase == RoulettePhase.ready) {
      angleDiff = 600;
      if (!audioSourceComponent.isPlaying) {
        audioSourceComponent.loop = true;
        audioSourceComponent.clip = roulettIdle;
        audioSourceComponent.Play();
      }
    } else if (roulettePhase == RoulettePhase.stop) {
      angleDiff -= 120f * Time.deltaTime;
    }

    if (roulettePhase == RoulettePhase.stop && angleDiff <= 0) {
      roulettePhase = RoulettePhase.calc;
      CalcBoost();
    }

    if (angleDiff <= 0)
      angleDiff = 0;

    angle -= angleDiff * Time.deltaTime;
    board.transform.eulerAngles = new Vector3(0, 0, angle);

  }

  /// <summary>
  /// Остановка рулетки
  /// </summary>
  [ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.StartFirstRunAnim))]
  public void StopRoulette(ExEvent.RunEvents.StartFirstRunAnim startRun) {
    roulettePhase = RoulettePhase.stop;
    audioSourceComponent.loop = false;
    audioSourceComponent.clip = roulettStop;
    audioSourceComponent.Play();
  }

  public void StopRotation() {
    roulettePhase = RoulettePhase.stop;
    audioSourceComponent.loop = false;
    audioSourceComponent.clip = roulettStop;
    audioSourceComponent.Play();
  }

  BoostType boost;

  /// <summary>
  /// Рассчет буста
  /// </summary>
  void CalcBoost() {
    float degre = board.transform.eulerAngles.z - 180;

    if (degre < 0) {
      degre = 360 - degre * -1;
    }

    boost = BoostType.none;

    for (int i = 0; i < rulletteType.Length; i++) {
      if (rulletteType[i].roundDegree > 0) {

        if (rulletteType[i].startRound < rulletteType[i].endRound) {
          if (rulletteType[i].startRound <= degre && rulletteType[i].endRound > degre) {
            boost = rulletteType[i].boost;
          }
        } else {
          if ((rulletteType[i].startRound <= degre && degre <= 360) || (rulletteType[i].endRound > degre && degre >= 0)) {
            boost = rulletteType[i].boost;
          }
        }
      }
    }

#if UNITY_EDITOR
    boost = BoostType.skate;
#endif
    boost = BoostType.none;

    RunnerController.Instance.StartRun(boost);
    
    //if (OnUse != null)
    //	OnUse(boost);

    StartDecor.Instance.SetActiveBoost(boost);

    //startObject.SetActiveBoost(boost);
    roulettePhase = RoulettePhase.end;
  }

}
