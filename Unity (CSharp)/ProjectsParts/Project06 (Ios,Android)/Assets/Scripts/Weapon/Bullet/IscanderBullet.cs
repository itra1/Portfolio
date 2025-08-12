using System.Collections;
using UnityEngine;

namespace Game.Weapon {

  public class IscanderBullet: Bullet {

    public GameObject boomPrefabs;
    private bool isMove = true;

    public CircleCollider2D colluder;

    public override void Move() {
      if (!isMove)
        return;
      transform.position += new Vector3(0.2f, -1f, 0) * speed * 2 * Time.deltaTime;
    }

    public override void OnEnable() {
      base.OnEnable();
      colluder.radius = 0.33f;
      isMove = true;
      PlayActivateAudio();

    }

    private IEnumerator TimeDeactive() {

      GameObject grenadeInst = Instantiate(boomPrefabs, new Vector3(transform.position.x, -3.25f, 0), Quaternion.identity);
      grenadeInst.SetActive(true);
      Destroy(grenadeInst, 2);

      colluder.radius = 2f;
      yield return new WaitForFixedUpdate();
      yield return new WaitForFixedUpdate();
      colluder.radius = 0.33f;
      DeactiveThis(null, true);
    }

    private void OnDrawGizmos() {
      Gizmos.DrawWireSphere(transform.position, 2);
    }

    public AudioBlock activeteAudioBlock;

    protected virtual void PlayActivateAudio() {
      if (activeteAudioBlock != null)
        activeteAudioBlock.PlayRandom(this);
      //if (damageAudio.Count == 0)
      //	return;
      //damageAudioBlock.
      //AudioManager.PlayEffects(damageAudio[Random.Range(0, damageAudio.Count)], AudioMixerTypes.effectPlay);
    }

    protected override void DeactiveThis(Enemy enemy, bool isGround = false) {
      if (!isGround)
        return;
      base.DeactiveThis(enemy, isGround);
    }

    /// <summary>
    /// Событие соприкосновение с землей
    /// </summary>
    public override void OnGround() {

      if (isMove) {
        isMove = false;
        StartCoroutine(TimeDeactive());

        BattleEventEffects.Instance.VisualEffect(BattleEffectsType.enyHitAfter5, transform.position);
      }
      //base.OnGround();
    }

  }

}