using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon {


  public class BottleDebris: Bullet {
    public List<GameObject> debrisSprite;
    private Vector3 startPosition;
    private float gravity = 3;
    public float groundLevelD;

    public override void OnEnable() {
      angleRotation = Random.Range(10, 180);
      rotationSpeed = Random.Range(30, 90);
      isMove = true;
      damageKoef = 1;
      rigidBody = GetComponent<Rigidbody2D>();
      startPosition = new Vector3(transform.position.x, groundLevelD, 0);
      velocity = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(0.3f, 0.6f), 0);
      if (transform.position.x < CameraController.leftPointX.x + 3f) {
        velocity.x = Random.Range(0f, 0.7f);
      }
    }

    private bool isMove;
    public override void Update() {

      if (!isMove)
        return;
      if (transform.position.y < startPosition.y && velocity.y < 0) {
        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
        isMove = false;
        return;
      }
      Move();
      Rotation();
    }

    public void SetSprite(int numer) {
      for (int i = 0; i < debrisSprite.Count; i++) {
        debrisSprite[i].SetActive(i == numer);
      }
    }
    public override void Rotation() {

      angleRotation += rotationSpeed * Time.deltaTime;
      rigidBody.MoveRotation(angleRotation);
    }

    public override void Move() {
      if (transform.position.x < CameraController.leftPointX.x + 2.8f) {
        isMove = false;
        gameObject.SetActive(false);
        return;
      }
      velocity.y -= gravity * Time.deltaTime;
      transform.position += velocity * speed * Time.deltaTime;
    }

    public override void OnTriggerEnter2D(Collider2D col) {

      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {

        try {
          if (col.GetComponent<Ekstrimalchik>() != null ||
              (col.GetComponent<BorodatoePevico>() != null && col.GetComponent<BorodatoePevico>().isJumping))
            return;
        } catch { }

        DamageEnemy(col.gameObject);
        DeactiveObject();
      }
      DamageBallonsTry(col);
    }

    private void OnTriggerStay2D(Collider2D col) {
    }

    private void DeactiveObject() {

      RaycastHit2D[] forDestroy = Physics2D.CircleCastAll(transform.position, 1.5f, Vector3.up);
      for (int i = 0; i < forDestroy.Length; i++) {
        if (forDestroy[i].collider.tag == "Debris")
          forDestroy[i].collider.gameObject.SetActive(false);

      }
      gameObject.SetActive(false);
    }

    public override void GetConfig() {
      base.GetConfig();
      _damageValue = wep.param1.Value * _damageValue;
    }

  }


}