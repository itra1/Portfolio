using UnityEngine;
using System.Collections.Generic;


public enum DeliveryType { none, post, letter }


public class BonusSpawner : MonoBehaviour {
    float timeLastGenerate;
    float timeIntervalGenerate;

    [System.Serializable]
    struct Bonuses {
        public BonusTypes type;
        public GameObject pref;
        [Range(0,1)]
        public float probability;
    }
  
  [SerializeField]
    Bonuses[] bonusesConfig;

    void Start()
    {
        PostController.OnGenerateBonus += GenerateBonus;
        timeLastGenerate = Time.time;
        timeIntervalGenerate = Random.Range(60f, 60f);
        StructProcessor();
    }

    void OnDestroy() {
        PostController.OnGenerateBonus -= GenerateBonus;
    }

	void Update () {
    
    if (Time.time > timeLastGenerate + timeIntervalGenerate) {
      if (!CopterController.instance.gameObject.activeInHierarchy)
        GenerateCopter();
      else timeLastGenerate += 2f;
    }
	}

    void GenerateCopter ()
    {
      timeLastGenerate = Time.time;
      timeIntervalGenerate = Random.Range(60f , 60f);
      CopterController.instance.SetTypeDeliverly(DeliveryType.post);
      CopterController.instance.gameObject.SetActive(true);

  }

    List<float> poolBonus;
    // Подготовка врагов для пула
    void StructProcessor() {
        //Создаем список объектов
        poolBonus = new List<float>();
        float sum1 = 0;
        poolBonus.Add(sum1);
        foreach(Bonuses enemy in bonusesConfig) {
            sum1 += enemy.probability;
            poolBonus.Add(sum1);
        }
        if(sum1 != 1f) poolBonus = poolBonus.ConvertAll(x => x / sum1);
    }

    void GenerateBonus(Vector3 positionGenerate) {
        int needNum = BinarySearch.RandomNumberGenerator(poolBonus);
        GameObject instant = Instantiate(bonusesConfig[needNum].pref, positionGenerate, Quaternion.identity) as GameObject;
        instant.transform.parent = transform;
        instant.SetActive(true);
    }


}
