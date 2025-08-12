using UnityEngine;

namespace Game.Weapon {

  public class HollyGrenadeWeapon: Bullet {
    public GameObject grenade;
    public GameObject funnel;
    public GameObject funnelGraphic;
    public bool funnelAcktive;
    private Vector2 razmer = new Vector2(3.9f, 2f);

    public GameObject boomPrefabs;

    public override void OnEnable() {
      base.OnEnable();

      funnel.GetComponent<BoxCollider2D>().size = razmer;


      grenade.SetActive(true);
      funnel.SetActive(false);
      funnelGraphic.SetActive(false);
      funnelAcktive = false;
      LightTween light = funnelGraphic.GetComponent<LightTween>();
      if (light != null)
        Destroy(light);

    }

    public override void Rotation() {
      if (!funnelAcktive) {
        angleRotation += rotationSpeed * Time.deltaTime;
        grenade.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angleRotation);
      }
    }

    public override void OnTriggerEnter2D(Collider2D col) {

      return;

      //if (funnelAcktive) return;
      //if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy" && col.gameObject.tag != "Voron")
      //{
      //  DamageEnemy(col.gameObject);
      //  funnel.SetActive(true);
      //  //DamageEnemy(col.gameObject);
      //  DeactiveThis();
      //} 
      //if (LayerMask.LayerToName(col.gameObject.layer) == "Bonuses")
      //{
      //  if (col.GetComponent<PostController>())
      //  {
      //    col.GetComponent<PostController>().DamagePlayer();
      //    funnel.SetActive(true);
      //    DeactiveThis();
      //  }
      //}
    }

    public override void OnGround() {
      if (funnelAcktive)
        return;
      //base.OnGround();
      transform.localEulerAngles = new Vector3(0f, 0f, 0f);
      funnelAcktive = true;

      funnel.SetActive(true);
      //funnelGraphic.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
      //funnelGraphic.SetActive(true);
      grenade.SetActive(false);

      GameObject grenadeInst = Instantiate(boomPrefabs, transform.position, Quaternion.identity);
      grenadeInst.SetActive(true);
      Destroy(grenadeInst, 2);

      Vanish();
    }

    public override void Move() {
      if (!funnelAcktive)
        base.Move();
    }

    protected override void DeactiveThis(Enemy enemy, bool isGround = false) {
      Debug.Log("DeactiveThis");
      base.DeactiveThis(enemy, isGround);
    }



    public override void CatapultStoneDamage(GameObject obj) {
      base.DeactiveThis(null);
    }

    public void Vanish() {

      LightTween.SpriteColorTo(funnelGraphic.GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 0), 1, 4, LightTween.EaseType.linear, gameObject, OnComplete);
    }

    private void OnComplete() {
      gameObject.SetActive(false);
    }

    #region Настройки

    public override void GetConfig() {
      base.GetConfig();

      razmer = new Vector2(wep.param1.Value * 2f, wep.param1.Value * 2);
      //razmer = new Vector2(float.Parse((string)config["RadiusDamage"]) * 2f, float.Parse((string)config["RadiusDamage"]) * 2);
    }
    #endregion
  }


}