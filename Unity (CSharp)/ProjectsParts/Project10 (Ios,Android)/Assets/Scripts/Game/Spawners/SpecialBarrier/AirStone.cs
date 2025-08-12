using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class AirStone : MonoBehaviour {

  private Vector3 velocity;

  private float angle;
  private float rotateSpeed;

  public GameObject particlesCloud;
  public GameObject particlesSland;
  public GameObject particlesSlandDark;

  public AudioClip damageClip;
  public GameObject[] particle;

	public Action OnDestroyEvent;
	
	private void OnEnable() {
		transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-180, 180));
		GetComponent<Rigidbody2D>().velocity = new Vector2(RunnerController.RunSpeed, 0);
		GetComponent<Rigidbody2D>().rotation = Random.Range(-1000, 1000);
	}
	
  void OnTriggerEnter2D(Collider2D obj) {
    if(LayerMask.LayerToName(obj.gameObject.layer) == "Ground") {
      GenParticle();

	    if (OnDestroyEvent != null) OnDestroyEvent();
	    OnDestroyEvent = null;

			AudioManager.PlayEffect(damageClip, AudioMixerTypes.runnerEffect);
      gameObject.SetActive(false);
    }

    if(LayerMask.LayerToName(obj.gameObject.layer) == "Player") {
      AudioManager.PlayEffect(damageClip, AudioMixerTypes.runnerEffect);
      obj.GetComponent<Player.Jack.PlayerController>().ThisDamage(WeaponTypes.airStone, Player.Jack.DamagType.live, 1, transform.position);
    }
  }

  void GenParticle() {
    foreach(GameObject partOne in particle) {
      GameObject part = Pooler.GetPooledObject(partOne.gameObject.name);
      part.SetActive(true);
      part.transform.position = partOne.transform.position;
      part.transform.parent = transform.parent;
			part.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5, 5), Random.Range(0, 5)),ForceMode2D.Impulse);
			//part.GetComponent<BodyParticle>().SetStartCoef(0);
			try {
				Destroy(part.GetComponentInChildren<SpriteRenderer>().GetComponent<LightTween>());
			} catch { }
			LightTween.SpriteColorTo(part.GetComponentInChildren<SpriteRenderer>(), new Color(1, 1, 1, 0), 5, 5);
      //Destroy(part, 10);
    }

    GameObject particl = Instantiate(particlesCloud.gameObject);
    particl.transform.position = transform.position;
    particl.transform.parent = transform.parent;
    particl.SetActive(true);
    Destroy(particl, 15);

    GameObject particlSland;

    if(Regions.type == RegionType.Crypt) {
      particlSland = Instantiate(particlesSlandDark.gameObject);
    } else
      particlSland = Instantiate(particlesSland.gameObject);

    particlSland.transform.position = transform.position;
    particlSland.transform.parent = transform.parent;
    particlSland.SetActive(true);
    Destroy(particlSland, 20);
  }

}
