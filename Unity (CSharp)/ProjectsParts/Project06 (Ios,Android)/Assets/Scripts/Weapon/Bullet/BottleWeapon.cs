using UnityEngine;

namespace Game.Weapon {

  /// <summary>
  /// Контроллер простой бутылки
  /// </summary>
  public class BottleWeapon: Bullet {
    public GameObject bottle;
    public GameObject debris;

    public override void OnEnable() {
      base.OnEnable();
      bottle.SetActive(true);
    }


    public override void CatapultStoneDamage(GameObject obj) {
      base.DeactiveThis(null);
    }

    public override void OnGround() {
      transform.localEulerAngles = new Vector3(0f, 0f, 0f);
      DeactiveThis(null);
      bottle.SetActive(false);
      Vanish();
    }

    protected override void DeactiveThis(Enemy enemu, bool isGround = false) {
      DebrisGenerate();
      base.DeactiveThis(enemu, isGround);
    }

    public override void OnTriggerEnter2D(Collider2D col) {
      if (!isActive || !isDamage)
        return;

      if (catapultaStonePush && col.tag == "CatapultStone")
        CatapultStonePush(col.gameObject);
      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
        DamageEnemy(col.gameObject);
        DamageObject(col);
        PlayDamageAudio();

        DeactiveThis(col.GetComponent<Enemy>());
      }
      if (LayerMask.LayerToName(col.gameObject.layer) == "Bonuses") {
        if (col.GetComponent<PostController>()) {
          col.GetComponent<PostController>().DamagePlayer();
          DamageObject(col);
          PlayDamageAudio();
          DeactiveThis(null);
        }
      }
      if (col.gameObject.tag == "Ballon") {
        try {
          col.GetComponent<CupidonBallonsHelper>().BalloneDestroy();
          PlayDamageAudio();
          DeactiveThis(null);
        } catch { }
      }
    }

    public void DebrisGenerate() {
      debris.transform.position = bottle.transform.position;
      debris.GetComponent<BottleDebris>().groundLevelD = groundLevel;
      for (int i = 0; i < 5; i++) {
        GameObject inst = PoolerManager.Spawn("Debris");
        inst.transform.position = transform.position;
        inst.SetActive(true);
        if (i == 0) {
          inst.GetComponent<BottleDebris>().SetSprite(0);
        } else {
          inst.GetComponent<BottleDebris>().SetSprite(Random.Range(1, 3));
        }

      }
    }

    public void Vanish() { }

    private void OnComplete() {
      gameObject.SetActive(false);
    }

    #region Настройки
    public override void GetConfig() {
      base.GetConfig();
    }

    #endregion

  }


}