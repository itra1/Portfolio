using UnityEngine;
using System.Collections;

public class CivilController : MonoBehaviour {

  [SerializeField]
  float speedX;

  Vector3 velocity;

  void Start() {
    InitMuve();
  }

  void Update() {
    Muve();
  }

  void LateUpdate() {
    MuveBonus();
  }

  void OnTriggerEnter2D(Collider2D col) {
    if(LayerMask.LayerToName(col.gameObject.layer) == "Bonuses" && bonusObject == null && col.GetComponent<BonusController>()) {

      GetBonus(col.gameObject);
      col.GetComponent<BonusController>().YouMuved();
    }
  }

  #region Muve

  void InitMuve() {
    velocity.x = speedX;
  }

  void Muve() {
    transform.position += velocity * Time.deltaTime;

    if(transform.position.x < CameraController.leftPointX.x - CameraController.distanseX / 2) Destroy(gameObject);
  }

  #endregion

  #region Работа с бонусами

  [SerializeField]
  Transform bonusObjectTransform;
  GameObject bonusObject;

  void GetBonus(GameObject bonus) {
    bonusObject = bonus;
    bonusObject.transform.position = bonusObjectTransform.position;
    bonusObject.transform.localEulerAngles = bonusObjectTransform.localEulerAngles;
  }

  void MuveBonus() {
    if(bonusObject == null) return;
    bonusObject.transform.position = bonusObjectTransform.position;

    if(bonusObject.transform.position.x <= PlayerController.Instance.gameObject.transform.position.x) {
      BonusToPlayer();
    }
  }

  void BonusToPlayer() {
    BattleManager.AddBonus(bonusObject.GetComponent<BonusController>().bonusType, bonusObject.GetComponent<BonusController>().bonusValue);
    Destroy(bonusObject);
  }

  #endregion
}
