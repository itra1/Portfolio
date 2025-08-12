using System;
using System.Collections.Generic;
using ExEvent;
using Spine.Unity;
using UnityEngine;

namespace Game.Weapon {


  public class Bear: EventBehaviour, IPlayerDamage {

    public Action OnDeactive;

    public enum Phase {
      forward,
      stayIdle,
      block,
      back
    }

    public Phase phase;

    public WeaponType weaponType;

    private Rigidbody2D rb;
    private Vector3 targetPosition;
    private float speed;
    private float healthStart = 5;
    private float health;

    private string runAnim = "walk";
    private string idleAnim = "idle";
    private string blockAnim = "block";
    private string hitAnimEvent = "hit";
    public SkeletonAnimation skeletonAnimation;
    private float damageKoef;
    private float minDamageBlock;
    private float minDamageBack;
    private float damageValue;
    private List<GameObject> contactList = new List<GameObject>();

    protected override void Awake() {
      base.Awake();
      GetConfig();
    }

    private void Start() { }

    private void OnEnable() {
      health = healthStart;
      speed = Math.Abs(speed);
      contactList.Clear();
      skeletonAnimation.Initialize(true);
      ChangePhase(Phase.forward, true);
      skeletonAnimation.state.Complete += OnEndAnim;
      skeletonAnimation.state.Event += OnEventAnim;
      rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable() {
      GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.9625344f);
      skeletonAnimation.state.Complete -= OnEndAnim;
      skeletonAnimation.state.Event -= OnEventAnim;

      contactList.ForEach(x => x.GetComponent<Enemy>().OnDead -= EnemyDead);
      contactList.Clear();
    }


    [ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
    private void BattleStart(BattleEvents.StartBattle startEvent) {
      gameObject.SetActive(false);
    }

    private void OnEndAnim(Spine.TrackEntry track) {
      if (track.ToString() == blockAnim) {
        ChangePhase(Phase.stayIdle);
      }
    }

    private void OnEventAnim(Spine.TrackEntry track, Spine.Event e) {

      if (e.data.name == hitAnimEvent) {
        Attack();
      }
    }

    private void CheckChangePhase() {

      if (contactList.Count > 0) {
        ChangePhase(Phase.stayIdle);
      }
    }

    private void ChangePhase(Phase phase, bool isFirst = false) {

      if (this.phase == phase && !isFirst)
        return;
      if (this.phase == Phase.back && !isFirst)
        return;

      if (phase == Phase.forward) {
        skeletonAnimation.timeScale = 1;
        SetAnimation(0, runAnim, true);
        GetComponent<BoxCollider2D>().enabled = true;
      }

      if (phase == Phase.block) {
        contactList.ForEach(x => x.GetComponent<Enemy>().OnDead -= EnemyDead);
        contactList.Clear();
        SetAnimation(0, blockAnim, false);
      }

      if (phase == Phase.stayIdle) {
        SetAnimation(0, idleAnim, true);
      }

      if (phase == Phase.back) {
        SetAnimation(0, runAnim, true);
        //GetComponent<BoxCollider2D>().offset = new Vector2(-10, 0.9625344f); // Что бы враги зафиксировали уход медведя
        //contactList.ForEach(x => x.GetComponent<Enemy>().damageList.RemoveAll(y => y = gameObject));
        //contactList.ForEach(x => x.GetComponent<Enemy>().OnDead -= EnemyDead);
        //contactList.Clear();
      }

      this.phase = phase;

    }

    private void Attack() {
      if (contactList.Count == 0)
        return;
      if (phase == Phase.forward)
        contactList.ForEach(x => x.GetComponent<Enemy>().Damage(gameObject, Mathf.Max(minDamageBlock, damageValue / contactList.Count), 0, 0));
      if (phase == Phase.back)
        contactList.ForEach(x => x.GetComponent<Enemy>().Damage(gameObject, Mathf.Max(minDamageBack, damageValue / contactList.Count), 0, 0));

    }

    public virtual void SetAnimation(int index, string animName, bool loop) {
      skeletonAnimation.state.SetAnimation(index, animName, loop);
    }

    private void Update() {

      if (phase == Phase.forward) {
        if (transform.position.x < -10)
          rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
        else
          rb.MovePosition(transform.position + (targetPosition - transform.position).normalized * speed * Time.deltaTime);
        //transform.position += Vector3.right * speed * Time.deltaTime;
        skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000);
        contactList.ForEach(x => x.transform.position += Vector3.right * speed * Time.deltaTime);

        if (transform.position.x >= targetPosition.x)
          ChangePhase(Phase.stayIdle);
      }
      if (phase == Phase.block) {
        rb.MovePosition(transform.position + Vector3.left * (speed / 2) * Time.deltaTime);
        //contactList.ForEach(x => x.transform.position += Vector3.right * speed * Time.deltaTime);
      }
      if (phase == Phase.back) {
        rb.MovePosition(transform.position + Vector3.left * speed * Time.deltaTime);
        skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000);

        if (transform.position.x < -13) {
          if (OnDeactive != null)
            OnDeactive();
          gameObject.SetActive(false);
        }

        //contactList.ForEach(x => x.transform.position += Vector3.right * speed * Time.deltaTime);
      }

    }

    public void Damage(float damageValue) {

      if (phase == Phase.back)
        return;

      health -= damageValue * (phase == Phase.forward ? damageKoef : 1);

      if (health <= 0) {
        ChangePhase(Phase.back);
        //contactList.ForEach(x => x.GetComponent<Enemy>().damageList.Remove(gameObject));
        //contactList.ForEach(x => x.GetComponent<Enemy>().damageList.Clear());
        //contactList.ForEach(x => x.GetComponent<Enemy>().SetPhase(EnemyPhases.run));
        //contactList.Clear();
        //GetComponent<BoxCollider2D>().enabled = false;
        //speed *= (-1);
        //skeletonAnimation.timeScale = -1;

      } else {

        if (phase == Phase.stayIdle) {
          ChangePhase(Phase.block);
        }

      }
    }

    public void SetTargetPosition(Vector3 newTarget) {
      targetPosition = new Vector3(newTarget.x,
        ((DecorationManager.Instance.loaderLocation.roadSize.max - DecorationManager.Instance.loaderLocation.roadSize.min) / 2 + DecorationManager.Instance.loaderLocation.roadSize.min), 0);
    }

    private void OnTriggerEnter2D(Collider2D oth) {

      if (LayerMask.LayerToName(oth.gameObject.layer) == "Enemy") {
        if (!contactList.Exists(x => x.gameObject == oth.gameObject)) {
          if (oth.gameObject.GetComponent<Enemy>().enemyType == EnemyType.MadamSufrajo) {
            ChangePhase(Phase.stayIdle);
          }
          if (oth.gameObject.GetComponent<Enemy>().enemyType == EnemyType.BorodatoePevico) {
            ChangePhase(Phase.stayIdle);
            oth.gameObject.GetComponent<BorodatoePevico>().JumpBack();
            return;
          }

          contactList.Add(oth.gameObject);
          oth.gameObject.GetComponent<Enemy>().OnDead += EnemyDead;
        }
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy") {
        if (contactList.Exists(x => x.gameObject == collision.gameObject)) {
          collision.gameObject.GetComponent<Enemy>().OnDead -= EnemyDead;
          contactList.Remove(collision.gameObject);
        }
      }
    }

    private void EnemyDead(Enemy enemy) {
      enemy.OnDead -= EnemyDead;
      contactList.Remove(enemy.gameObject);
    }

    #region Настройки

    protected Dictionary<string, object> xmlConfig;

    public bool DamageReady {
      get { return phase != Phase.back; }
    }

    public virtual void GetConfig() {

      Configuration.Weapon wep = GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)weaponType);

      healthStart = wep.param2.Value;
      speed = wep.param1.Value;
      damageValue = wep.damage.Value;
      damageKoef = wep.param3.Value;
      minDamageBlock = wep.param4.Value;
      minDamageBack = wep.param5.Value;
      transform.localScale = new Vector3(wep.param6.Value, wep.param6.Value, wep.param6.Value);
    }

    #endregion

  }

}