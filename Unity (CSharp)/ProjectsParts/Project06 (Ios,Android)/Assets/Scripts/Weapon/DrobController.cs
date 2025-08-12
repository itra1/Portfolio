using UnityEngine;

namespace Game.Weapon {


  public class DrobController: Bullet {

    public GameObject spriteGraphic;
    private Vector3 pointStart;
    private float distanceShoot;
    private float angleAttack;
    public TrailRenderer trail;
    public override void OnEnable() {
      base.OnEnable();
      pointStart = transform.position;
      if (trail != null)
        trail.Clear();

    }


    public override void Update() {
      if (!isActive)
        return;
      //base.Update();
      Move();
      if (Vector3.Distance(pointStart, transform.position) > distanceShoot) {

        DeactiveThis(null);
      }
    }

    private float speedDrob;
    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {

      float angle = Random.Range(-1 * angleAttack, angleAttack);
      float anglRadian = angle * (Mathf.PI / 180);

      Vector2 alterTapEnd = tapEnd - tapStart;
      tapEnd = new Vector3(alterTapEnd.x * Mathf.Cos(anglRadian) - alterTapEnd.y * Mathf.Sin(anglRadian),
                          alterTapEnd.x * Mathf.Sin(anglRadian) + alterTapEnd.y * Mathf.Cos(anglRadian),
                          0) + tapStart;


      velocity = (tapEnd - tapStart).normalized * Random.Range(speedDrob - 0.5f, speedDrob + 0.5f);
      //base.Shot(tapStart, tapEnd);
    }

    public override void Move() {
      rig.MovePosition(transform.position + velocity.normalized * Time.deltaTime * speedDrob);
      //transform.position += velocity.normalized * Time.deltaTime * speedDrob;
    }

    public override void Rotation() {
      angleRotation = Vector3.Angle(Vector3.up, velocity);
      transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -angleRotation + 90);
    }

    public override void GetConfig() {
      base.GetConfig();

      distanceShoot = wep.param4.Value;
      angleAttack = wep.param2.Value;
      speedDrob = wep.param3.Value;

      //if (config.ContainsKey("partOfDisplay"))
      //  partOfDisplay = float.Parse((string)config["partOfDisplay"]); // = 0.35f
      //if (config.ContainsKey("damageValue"))
      //  damageValue = int.Parse((string)config["damageValue"]); // = 1
      //if (config.ContainsKey("angleAttack"))
      //  angleAttack = float.Parse((string)config["angleAttack"]) / 2;
      //if (config.ContainsKey("speedDrob"))
      //  speedDrob = float.Parse((string)config["speedDrob"]) / 2;
    }

    // 



  }

}