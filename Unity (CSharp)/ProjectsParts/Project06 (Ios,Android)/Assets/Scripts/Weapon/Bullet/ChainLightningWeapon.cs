using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Weapon {

  public class ChainLightningWeapon: Bullet {

    public OneChaineLight miniLight;
    public OneChaineLight middleLight;
    public OneChaineLight bigLight;

    public ParticleSystem enemyLight;
    private List<ParticleSystem> enemyLightInstance = new List<ParticleSystem>();
    private List<OneChaineLight> middleLightList = new List<OneChaineLight>();

    private Vector3 _pointStart;
    private bool isMove;

    public LayerMask enemyLayer;

    public override void OnEnable() {
      base.OnEnable();
      damageListEnemy.Clear();
      sourceEnemyList.Clear();
      isMove = true;
      _pointStart = transform.position;
      graphic.SetActive(true);
    }

    public override void Update() {
      //base.Update();
      if (isMove)
        Move();
      //if (Vector3.Distance(_pointStart, transform.position) > distanceShoot) {
      //	DeactiveThis();
      //}
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {
      velocity = (tapEnd - tapStart).normalized;
    }

    public override void Move() {
      transform.position += velocity.normalized * Time.deltaTime * speed;
    }

    public override void OnTriggerEnter2D(Collider2D col) {
      if (!isMove)
        return;

      if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
        isMove = false;
        graphic.SetActive(false);
        DamageEnemy(col.gameObject);
        PlayDamageAudio();
        sourceEnemyList.Add(col.GetComponent<Enemy>());
        damageListEnemy.Add(col.GetComponent<Enemy>());
        //DeactiveThis();
        StartCoroutine(LightChaine());
      }
    }

    private List<Enemy> damageListEnemy = new List<Enemy>();

    private List<Enemy> sourceEnemyList = new List<Enemy>();

    private IEnumerator LightChaine() {

      while (sourceEnemyList.Count > 0) {
        yield return new WaitForSeconds(0.5f);
        List<Enemy> sourceEnemyListTmp = new List<Enemy>(sourceEnemyList);
        sourceEnemyList.Clear();
        sourceEnemyListTmp.ForEach(FindEnemy);
      }
      DeactiveThis(null);

    }

    private void FindEnemy(Enemy sourceEnemy) {

      // смотрим вправо
      Collider2D[] targetEnemy = Physics2D.OverlapAreaAll(sourceEnemy.transform.position, sourceEnemy.transform.position + new Vector3(3, 1.5f, 0), enemyLayer);

      Dictionary<float, Enemy> targetEnemyDict = new Dictionary<float, Enemy>();

      for (int i = 0; i < targetEnemy.Length; i++) {
        float posX = ((targetEnemy[i] as PolygonCollider2D).points.OrderBy(x => x.x).ToArray()[0] + (Vector2)targetEnemy[i].transform.position).x;
        if (!targetEnemyDict.ContainsKey(posX))
          targetEnemyDict.Add(posX, targetEnemy[i].GetComponent<Enemy>());
      }

      targetEnemyDict = targetEnemyDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

      foreach (var targetKey in targetEnemyDict.Keys) {
        if (targetEnemyDict[targetKey] == sourceEnemy)
          continue;
        if (!damageListEnemy.Contains(targetEnemyDict[targetKey])) {
          damageListEnemy.Add(targetEnemyDict[targetKey]);
          sourceEnemyList.Add(targetEnemyDict[targetKey]);
          DamageEnemy(targetEnemyDict[targetKey].gameObject);
          Light(sourceEnemy.gameObject, targetEnemyDict[targetKey].gameObject);
          PlayDamageAudio();
          break;
        }
      }

      targetEnemy = Physics2D.OverlapAreaAll(sourceEnemy.transform.position, sourceEnemy.transform.position - new Vector3(3, 1.5f, 0), enemyLayer);

      targetEnemyDict.Clear();

      for (int i = 0; i < targetEnemy.Length; i++) {
        float posX = ((targetEnemy[i] as PolygonCollider2D).points.OrderBy(x => x.x).ToArray()[0] + (Vector2)targetEnemy[i].transform.position).x;
        if (!targetEnemyDict.ContainsKey(posX))
          targetEnemyDict.Add(posX, targetEnemy[i].GetComponent<Enemy>());
      }
      targetEnemyDict = targetEnemyDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

      foreach (var targetKey in targetEnemyDict.Keys) {
        if (targetEnemyDict[targetKey] == sourceEnemy)
          continue;
        if (!damageListEnemy.Contains(targetEnemyDict[targetKey])) {
          damageListEnemy.Add(targetEnemyDict[targetKey]);
          sourceEnemyList.Add(targetEnemyDict[targetKey]);
          DamageEnemy(targetEnemyDict[targetKey].gameObject);
          PlayDamageAudio();
          Light(sourceEnemy.gameObject, targetEnemyDict[targetKey].gameObject);
          break;
        }
      }
    }

    private void Light(GameObject sourceEnemy, GameObject target) {

      OneChaineLight instPart = middleLightList.Find(x => !x.gameObject.activeInHierarchy);
      if (instPart == null) {
        instPart = Instantiate(middleLight);
        instPart.transform.SetParent(middleLight.transform.parent);
      }

      instPart.SetLight(sourceEnemy.transform, target.transform, 0.2f);
      instPart.gameObject.SetActive(true);

    }

    private IEnumerator Light(OneChaineLight light) {
      yield return new WaitForSeconds(0.5f);
      light.gameObject.SetActive(false);
    }

    protected override void DamageEnemy(GameObject enemy) {
      base.DamageEnemy(enemy);

      ParticleSystem instPart = enemyLightInstance.Find(x => !x.gameObject.activeInHierarchy);

      if (instPart == null) {
        instPart = Instantiate(enemyLight);
        instPart.transform.SetParent(enemyLight.transform.parent);
      }
      StartCoroutine(EnemyLight(enemy, instPart));
    }

    private IEnumerator EnemyLight(GameObject enemy, ParticleSystem particle) {

      particle.gameObject.SetActive(true);
      ParticleSystem.ShapeModule sm = particle.shape;

      sm.meshRenderer = enemy.GetComponent<Enemy>().skeletonAnimation.GetComponent<MeshRenderer>();
      yield return new WaitForSeconds(0.5f);
      particle.gameObject.SetActive(false);

    }

  }


}