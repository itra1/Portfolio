using UnityEngine;
using System.Collections;

/// <summary>
/// Контроллер светильников
/// </summary>
public class TorchController : MonoBehaviour {

	public GameObject fire;
	public GameObject smoke;
	public SpriteRenderer fireSpriteLight;
	//Transform cameraObj;
	public LayerMask playerLayer;
	private bool isActive;

	public AudioClip torch;

	void OnEnable() {
		fireSpriteLight.sortingLayerName = fire.GetComponent<SpriteRenderer>().sortingLayerName;
		fireSpriteLight.sortingOrder = 1;
		isActive = true;
		fire.SetActive(true);
		smoke.SetActive(false);
		fireSpriteLight.gameObject.SetActive(true);
	}
	
	private void OnTriggerEnter2D(Collider2D other) {

		if (!isActive) return;

		if (!other.tag.Equals("Player")) return;

		isActive = false;
		fire.SetActive(false);
		smoke.SetActive(true);

		fireSpriteLight.gameObject.SetActive(false);

		if (torch != null)
			AudioManager.PlayEffect(torch, AudioMixerTypes.runnerEffect);

		Questions.QuestionManager.ConfirmQuestion(Quest.putOutTorch, 1, other.transform.position);

	}

}
