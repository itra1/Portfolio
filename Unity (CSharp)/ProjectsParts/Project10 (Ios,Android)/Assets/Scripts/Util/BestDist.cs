using UnityEngine;

public class BestDist : MonoBehaviour {


	public delegate void NewRecord();
	public static event NewRecord newRecord;

	[SerializeField] AudioClip contactClip;
	bool act = true;
	[SerializeField] SpriteRenderer backBg;
	public GameObject panelPhoto;
	public TextMesh tableText;
	public MeshRenderer mesh;
	public GameObject aura;
	public bool myBest;

	public GameObject panelNoPhoto;
	public TextMesh tableTextNoPhoto;

	public bool bestDist;

	void OnEnable() {
		tableText.GetComponent<MeshRenderer>().sortingLayerID = backBg.sortingLayerID;
		tableText.GetComponent<MeshRenderer>().sortingOrder = 1;
		if (tableTextNoPhoto != null) {
			tableTextNoPhoto.GetComponent<MeshRenderer>().sortingLayerID = backBg.sortingLayerID;
			tableTextNoPhoto.GetComponent<MeshRenderer>().sortingOrder = 1;
		}
		mesh.sortingOrder = 1;
		mesh.sortingLayerID = backBg.sortingLayerID;
	}

	void Update() {
		if (transform.position.x < CameraController.displayDiff.leftDif(2))
			Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D oth) {

		if (act == false)
			return;

		if (oth.tag == "Player") {
			act = false;
			if (bestDist) {
				AudioManager.PlayEffect(contactClip, AudioMixerTypes.runnerEffect);
				Questions.QuestionManager.fbOvertakeFriend(oth.transform.position);
			}

			if (myBest) {
				//Questions.QuestionManager.addNewMyRecords(oth.transform.position);
				Questions.QuestionManager.ConfirmQuestion(Quest.newMyRecords, 1, oth.transform.position);
				if (newRecord != null)
					newRecord();
			}
		}

	}

	public void SetBestDist() {
		aura.SetActive(true);
		bestDist = true;
	}

}
