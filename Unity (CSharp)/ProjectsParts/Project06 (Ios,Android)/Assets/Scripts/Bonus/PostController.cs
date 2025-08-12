using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Контроллер посылки
/// </summary>
public class PostController : MonoBehaviour , IPlayerDamage {

  float enemyDamage;

  public delegate void GenerateBonus(Vector3 position);
  public static event GenerateBonus OnGenerateBonus;

  void Awake() {
    GetConfig();
    InitDurable();
  }

  void OnEnable() {
    InitDurable();
  }

  void Update() {
    Muve();
  }

  #region Muve
  Vector3 velocity;

  [SerializeField]
  float gravity;
  [SerializeField]
  float horizontalFriction;
  [SerializeField]
  bool isFree;

  [SerializeField]
  float groundLevel;

  public void GoFree(float speedX) {
    isFree = true;
    velocity.x = speedX;
  }

  void Muve() {
    if(!isFree) return;

    velocity.y -= gravity * Time.deltaTime;
    velocity.x = Mathf.Max(0 , velocity.x - horizontalFriction * Time.deltaTime);

    transform.position += velocity * Time.deltaTime;

    if(transform.position.y <= groundLevel) {
      isFree = false;
      transform.position = new Vector3(transform.position.x , groundLevel , transform.position.z);
    }
  }
  #endregion
  
  #region Настройки
  
  void GetConfig() {
    durableMax = 100;
    enemyDamage = 100;
    
  }

  #endregion

  void OnTriggerEnter2D(Collider2D col) {
    if(!isFree) return;
        
    if(LayerMask.LayerToName(col.gameObject.layer) == "Enemy" && col.gameObject.tag != "Voron") {
        
        if (col.GetComponent<Enemy>()) {
            col.GetComponent<Enemy>().Damage(gameObject, enemyDamage);

      }
    }
    
}

  #region Прочность

  float durableMax;
  float durable;                                      // Запас прочности
  bool _isOpen;

  public bool isOpen {
    get { return _isOpen; }
  }
	public bool DamageReady {
		get { return true; }
	}

	public delegate void DestroyThis(GameObject target);
  public event DestroyThis destroyThis;

  void InitDurable() {
    durable = durableMax;
  }

  //public void DamageEnemy(float damage) {
  //  if(_isOpen) return;

  //  durable -= damage;

  //  if(durable <= 0) {
  //    Destroy(gameObject);
      


  //  }
  //}

  public void DamagePlayer() {

     _isOpen = true;
    if(OnGenerateBonus != null) OnGenerateBonus(transform.position);
    Destroy(gameObject);

  }

  void OnDestroy() {
    if(destroyThis != null) destroyThis(gameObject);
  }

	public void Damage(float value) {
		durable -= value;

		if (durable <= 0) {
			Destroy(gameObject);
		}
	}

	#endregion
}
