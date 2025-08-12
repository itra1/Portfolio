using UnityEngine;
using System.Collections;

public class MessageController : MonoBehaviour {

  public GameObject letter;
  public GameObject message;

  void Awake() {
    //GetConfig();
  }

  void OnEnable() {
    letter.SetActive(true);
    message.SetActive(false);
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
    velocity.x = Mathf.Max(0, velocity.x - horizontalFriction * Time.deltaTime);

    transform.position += velocity * Time.deltaTime;

    if(transform.position.y <= CameraController.rightPoint.y + 2) {
      letter.SetActive(false);
      message.SetActive(true);
    }

    if(transform.position.y <= groundLevel) {
      isFree = false;
      Destroy(gameObject);

    }
  }
  #endregion

  public TextMesh textMessage;

  public void WriteMessage(string message) {
    textMessage.text = message;
  }
  #region Настройки
  /*
  [SerializeField]
  string configLink;

  Dictionary<string, object> config;
  void GetConfig()
  {
    config = GameDesign.GetConfig(configLink);
    durableMax = float.Parse((string)config["Durable"]);
    enemyDamage = float.Parse((string)config["DamageEnemy"]);

  }
  */
  #endregion




}


