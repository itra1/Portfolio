using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер Огненного шара
/// </summary>

public class Meteor : MonoBehaviour {

	[SerializeField] GameObject meteorBoom;
    [SerializeField] GameObject meteorFire;
    [SerializeField] GameObject scar;

    [SerializeField] float damage;

    void OnEnable() {
        meteorBoom.SetActive(false);
        LightTween.Stop(gameObject, true);
        scar.GetComponent<SpriteRenderer>().color = Color.white;
        scar.transform.localScale = Vector3.zero;
        scar.gameObject.SetActive(true);

        meteorFire.SetActive(true);
        meteorFire.GetComponent<MagicHitHelper>().Hit += OnMeteorHit;
    }

    public void OnMeteorHit() {
        meteorBoom.gameObject.SetActive(true);
        LightTween.ScaleTo(scar, Vector3.one, 0.1f, 0, LightTween.EaseType.easeOutBack);
        LightTween.SpriteColorTo(scar.GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 0), 1, 4, LightTween.EaseType.linear, this.gameObject, OnComplete);

        StartCoroutine(ColliderHelper());
    }

    IEnumerator ColliderHelper() {
        foreach(Collider2D coll in GetComponentsInChildren<Collider2D>())
            coll.enabled = true;

        yield return new WaitForSeconds(0.3f);
        foreach(Collider2D coll in GetComponentsInChildren<Collider2D>())
            coll.enabled = false;

        yield return new WaitForSeconds(0.5f);
        /*if(OnFinish != null) {
            OnFinish(spell);
            OnFinish = null;
        }*/
    }

    void OnComplete() {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if(LayerMask.LayerToName(coll.gameObject.layer) == "Enemy" ) {
            Enemy enemy = coll.GetComponent<Enemy>();
            enemy.Damage(gameObject, damage);
            //enemy.AddEffects(spell.CreateAxeEffects(false));
        }
    }

}
