using UnityEngine;

public class BarrierController : MonoBehaviour {

	public LayerMask groundMask;
	public LayerMask groundTopMask;
	public int playerDamage;
	public float enemyDamager;
	public GameObject poof;
	public GameObject particlesCloud;
	public GameObject particlesSland;

	public AudioClip damageClip;

	public GameObject[] particle;

	public delegate void BarrierDestroy();
	public BarrierDestroy OnBarrierDestroy;

	void OnEnable() {
		SetDown();
	}
	/// <summary>
	/// Позиционирование к потолку
	/// </summary>
	public void SetTop() {
		transform.localScale = new Vector3(1, -1, 1);
		transform.position = new Vector3(transform.position.x, CameraController.displayDiff.topDif(1), 0);
	}

	/// <summary>
	/// Позиционирование на земле
	/// </summary>
	public void SetDown() {
		transform.localScale = new Vector3(1, 1, 1);
		RaycastHit2D[] grnd = Physics2D.RaycastAll(transform.position + Vector3.up / 2, Vector3.down * 7, groundMask);

		if (grnd.Length > 0) {

			foreach (RaycastHit2D one in grnd) {
				if (LayerMask.LayerToName(one.transform.gameObject.layer) == "Ground")
					transform.position = new Vector3(transform.position.x, one.transform.position.y, 0);
			}

		}
		else
			gameObject.SetActive(false);
	}

	public void DestroyThis() {

		// Для туториола
		//TutorialController.FailedTutor();

		GenParticle();

		AudioManager.PlayEffect(damageClip, AudioMixerTypes.runnerEffect);

		if (OnBarrierDestroy != null)
			OnBarrierDestroy();

		OnBarrierDestroy = null;

		gameObject.SetActive(false);
	}

	void GenParticle() {
		foreach (GameObject partOne in particle) {
			GameObject part = Instantiate(partOne.gameObject);
			part.SetActive(true);
			part.transform.position = partOne.transform.position;
			part.transform.parent = transform.parent;
			part.GetComponent<BodyParticle>().SetStartCoef(0);
		}

		GameObject particl = Instantiate(particlesCloud.gameObject);
		particl.transform.position = transform.position;
		particl.transform.parent = transform.parent;
		particl.SetActive(true);
		Destroy(particl, 1);
		GameObject particlSland = Instantiate(particlesSland.gameObject);
		particlSland.transform.position = transform.position;
		particlSland.transform.parent = transform.parent;
		particlSland.SetActive(true);
		Destroy(particlSland, 20);
	}
}
