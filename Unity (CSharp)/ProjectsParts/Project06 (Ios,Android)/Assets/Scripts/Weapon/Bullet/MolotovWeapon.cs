using UnityEngine;

namespace Game.Weapon {


  public class MolotovWeapon: Bullet {

    public delegate void OnDestroyDelegate(Vector3 position);
    public OnDestroyDelegate OnDestroyNow;

    public GameObject boomPrefabs;

    private void OnDisable() {
      //WeaponGenerator.instance.GetMolotovFlame(new Vector3(transform.position.x , groundLevel , 0));
    }

    public override void OnTriggerEnter2D(Collider2D col) {
      if (catapultaStonePush && col.tag == "CatapultStone")
        CatapultStonePush(col.gameObject);
      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {

        if (col.GetComponent<DiskoZorb>() != null) {
          col.GetComponent<DiskoZorb>().SetBound(gameObject);
          SetRebound(col);
          return;
        }

        DamageEnemy(col.gameObject);
        DeactiveThis(col.GetComponent<Enemy>());
      }
      if (LayerMask.LayerToName(col.gameObject.layer) == "Bonuses") {
        if (col.GetComponent<PostController>()) {
          col.GetComponent<PostController>().DamagePlayer();
          gameObject.SetActive(false);
        }
      }
    }

    protected override void DeactiveThis(Enemy enemy, bool isGround = false) {
      SearchEnemy();
      base.DeactiveThis(enemy, isGround);

      if (OnDestroyNow != null)
        OnDestroyNow(new Vector3(transform.position.x, groundLevel, 0));
      OnDestroyNow = null;

      GameObject weapon = PoolerManager.Spawn("MolotovFlame");
      //weapon.transform.parent = transform;
      weapon.transform.position = transform.position;
      weapon.SetActive(true);


      GameObject grenadeInst = Instantiate(boomPrefabs, (enemy != null ? enemy.transform.position : transform.position), Quaternion.identity);
      grenadeInst.SetActive(true);
      Destroy(grenadeInst, 2);

    }

    private float timeWorkFlameOfFire;
    private float periodWorKFlameOfFire;
    private float damageFlameOfFire;

    public LayerMask maskEnemy;
    public void SearchEnemy() {
      RaycastHit2D[] forFire = Physics2D.CircleCastAll(transform.position, 2f, Vector2.up, 2f, maskEnemy);
      if (forFire == null)
        return;

      int enemyCount = 0;

      for (int i = 0; i < forFire.Length; i++) {
        if (forFire[i].collider.GetComponent<Enemy>()) {
          enemyCount++;
          forFire[i].collider.GetComponent<Enemy>().SetBurn(timeWorkFlameOfFire, periodWorKFlameOfFire, damageFlameOfFire);
        }
      }
      if (enemyCount >= 5) {
        BattleEventEffects.Instance.VisualEffect(BattleEffectsType.molotov5, transform.position, this);
      }

    }


    public override void CatapultStoneDamage(GameObject obj) {
      base.DeactiveThis(null);
    }

    #region Настройки


    public override void GetConfig() {
      base.GetConfig();

      timeWorkFlameOfFire = wep.param1.Value;
      periodWorKFlameOfFire = wep.param2.Value;
      damageFlameOfFire = wep.param3.Value;

      //if (config.ContainsKey("timeWorkFlameOfFire"))
      //  timeWorkFlameOfFire = float.Parse((string)config["timeWorkFlameOfFire"]);
      //if (config.ContainsKey("periodWorKFlameOfFire"))
      //  periodWorKFlameOfFire = float.Parse((string)config["periodWorKFlameOfFire"]);
      //if (config.ContainsKey("damageFlameOfFire"))
      //  damageFlameOfFire = float.Parse((string)config["damageFlameOfFire"]);

    }

    #endregion
  }

}