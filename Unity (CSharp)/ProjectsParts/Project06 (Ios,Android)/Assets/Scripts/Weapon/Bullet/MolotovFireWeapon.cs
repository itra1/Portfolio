using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Weapon {

  public class MolotovFireWeapon: Bullet {

    public SpriteRenderer sprite;
    public SortingGroup sortingGroup;

    /// <summary>
    /// Спосок врагов в зоне действия
    /// </summary>
    private List<DamageObg> damageObjList = new List<DamageObg>();

    /// <summary>
    /// Промежутки времени между повторным нанесением дамага
    /// </summary>
    private float damagePeriodTime;

    /// <summary>
    /// Время, в течении которого наносятся повреждения
    /// </summary>
    private float damageAllTime;

    public Transform[] flames;

    public override void OnEnable() {
      base.OnEnable();

      //sprite.sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000);
      //spriteList.ForEach(x=>x.sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000));
      sortingGroup.sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000);

      for (int i = 0; i < flames.Length; i++) {
        float scale = Random.Range(0.7f, 1.6f);
        flames[i].transform.localScale = new Vector3(scale, scale, scale);

      }

      Invoke("DeactiveObject", damageAllTime);
    }

    public override void Update() { }

    /// <summary>
    /// Деактивация объекта
    /// </summary>
    private void DeactiveObject() {
      gameObject.SetActive(false);
    }

    public override void OnTriggerEnter2D(Collider2D col) { }

    private void OnTriggerStay2D(Collider2D col) {

      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {

        if (damageObjList.Exists(x => x.objId == col.gameObject.GetInstanceID() && x.lastDamageTime + damagePeriodTime <= Time.time)) {
          damageObjList.RemoveAll(x => x.objId == col.gameObject.GetInstanceID());
        }
        if (!damageObjList.Exists((x => x.objId == col.gameObject.GetInstanceID()))) {
          DamageObg struktura = new DamageObg();
          struktura.lastDamageTime = Time.time;
          struktura.objId = col.gameObject.GetInstanceID();
          damageObjList.Add(struktura);
          DamageEnemy(col.gameObject);
        }

      }
    }

    public override void GetConfig() {
      base.GetConfig();

      damageAllTime = wep.param1.Value;
      damagePeriodTime = wep.param2.Value;
      _damageValue = wep.param3.Value;

      //damageAllTime = float.Parse(( string )config["DamageTime"]);
      //damagePeriodTime = float.Parse(( string )config["DamagePeriod"]);
    }

  }


}