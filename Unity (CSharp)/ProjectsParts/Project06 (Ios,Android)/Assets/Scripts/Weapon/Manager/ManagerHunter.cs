using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon {



  /// <summary>
  /// Управляющий контроллер топата
  /// </summary>
  public class ManagerHunter: WeaponAssistant {

    public float damage;
    private float timeShootReady;
    private float timeTargetAcktive;
    private bool isShoot;
    private List<Enemy> damageEnemy = new List<Enemy>();

    public GameObject targetObject;
    public LayerMask targetMask;

    public override void Inicialized() {
      base.Inicialized();
      timeTargetAcktive = -1f;

      targetObject.SetActive(isSelected);
      TargetPosition();
    }

    protected override void DeInicialized() {
      base.DeInicialized();
      targetObject.SetActive(isSelected);
    }

    protected override void OnClickDown(Vector3 position) {
      if (!CheckReadyShoot(PlayerController.Instance.ShootPoint, position))
        return;
      if (isShoot)
        return;

      damageEnemy.Clear();

      isShoot = true;
      PlayShootHunterStartAudio();
      StartCoroutine(ShootCor());
    }

    protected override void Update() {
      base.Update();

      if (isSelected) {
        TargetPosition();
      }
    }

    public override bool CheckReadyShoot(Vector3 tapStart, Vector3 tapEnd) {
      if (tapStart.x > tapEnd.x || tapEnd.x - tapStart.x < 0.5f)
        return false;
      return base.CheckReadyShoot(tapStart, tapEnd);
    }

    private void TargetPosition() {
      if (Time.time > timeTargetAcktive) {

        targetObject.SetActive(true);
      }
      targetObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

    }


    public AudioBlock playShootAudioBlock;

    protected void PlayShootHunterStartAudio() {
      playShootAudioBlock.PlayRandom(this);
    }

    private IEnumerator ShootCor() {
      Debug.Log("timeShootReady  " + timeShootReady);

      yield return new WaitForSeconds(timeShootReady);
      RaycastHit2D[] allObject = Physics2D.LinecastAll(PlayerController.Instance.ShootPoint, PlayerController.Instance.ShootPoint + ((pointerDown - PlayerController.Instance.ShootPoint).normalized * 30));
      if (allObject.Length > 0) {
        for (int i = 0; i < allObject.Length; i++) {
          if (LayerMask.LayerToName(allObject[i].collider.gameObject.layer) == "Enemy") {

            if (!damageEnemy.Contains(allObject[i].collider.gameObject.GetComponent<Enemy>())) {
              damageEnemy.Add(allObject[i].collider.gameObject.GetComponent<Enemy>());
              BattleEventEffects.Instance.VisualEffect(BattleEffectsType.hunterObrezShoot, allObject[i].collider.transform.position, this);
            }
            allObject[i].collider.gameObject.GetComponent<Enemy>().Damage(gameObject, damage);
          }

        }
      }
      isShoot = false;
      base.OnShoot(Vector3.zero, pointerDown);

      PlayReload();
      OnShootComplited();
    }

    public override void GetConfig() {
      base.GetConfig();

      damage = wepConfig.damage.Value;
      timeShootReady = wepConfig.param1.Value;
    }
  }

}