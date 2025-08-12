using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon {

  public class VarganichBullet: Bullet {

    public GameObject lightPrefab;
    public GameObject rainPrefab;
    public List<ParticleSystem> particles;
    private float _rainTime;

    private Phase _phase;

    public enum Phase {
      light,
      rain
    }

    public override void OnEnable() {
      base.OnEnable();
      _phase = Phase.light;
      lightPrefab.SetActive(true);
      rainPrefab.SetActive(false);
      speed = 1;
      lightPrefab.GetComponent<LightningWeapon>().OnDestroyBullet = () => {
        _phase = Phase.rain;
        velocity = Vector3.right;
        rainPrefab.SetActive(true);
        StartCoroutine(RainUse());
        //lightPrefab.SetActive(false);
      };
    }

    private IEnumerator RainUse() {
      float startTime = Time.time;
      while (startTime + _rainTime > Time.time)
        yield return null;
      DeactiveThis(null);
    }

    private void OnDisable() {
      lightPrefab.SetActive(false);
      rainPrefab.SetActive(false);
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) { }

    public override void Move() {
      if (_phase == Phase.rain) {

        velocity = transform.position.x < 7 ? Vector3.right : Vector3.zero;
        transform.position += velocity.normalized * speed * Time.deltaTime;
      }
    }
    public override void OnGround() { }

    public override void GetConfig() {
      base.GetConfig();
      speed = wep.param1.Value;
      _rainTime = wep.param2.Value;

    }
  }


}