using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour, IOnPlayerTrigger {

	private Transform player;										// Плеер
	public AudioClip[] audioClip;								// Аудио клип
	
	[HideInInspector]
	public bool toPlayer;												// Флаг движения к игроку
	private float speed;												// Скорость

	private Vector3 velocity;										// Вектор движения
	private float groundPointY;
	private float timeWait;
	private Transform countPosition;
	
	private int nomination;											// Номинал ценности монеты
	public Animator graphAnim;
	public Action OnGetPlayer;
	private Coroutine destroyCorotine;
	
	enum MapPhases { none, start, stay, toCount, end }
	MapPhases mapPhase;

	void OnEnable() {
		toPlayer = false;
		speed = 0;
		
	}
		
	void OnDisable() {
		StopAllCoroutines();
//#if UNITY_EDITOR
//		SetNomination(1);
//#endif
	}

	void Update() {
		if(mapPhase != MapPhases.none) {
			MoveMap();
			return;
		}

		if(toPlayer) {
			if(speed < (RunnerController.RunSpeed + 20))	speed += (RunnerController.RunSpeed + 20) * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
		}
	}
	
	void MoveMap() {
		if(mapPhase == MapPhases.start) {
			velocity.y -= 3 * Time.deltaTime;
			transform.position += velocity * Time.deltaTime;

			if(velocity.y < 0 && transform.position.y <= groundPointY) {
				mapPhase = MapPhases.stay;
				timeWait = Time.time + 1f;
			}
		}

		if(mapPhase == MapPhases.stay && timeWait <= Time.time)
			mapPhase = MapPhases.toCount;

		if(mapPhase == MapPhases.toCount || mapPhase == MapPhases.end) {
			Vector3 tmpPos = transform.position;
			transform.position += (countPosition.position - tmpPos).normalized * 6 * Time.deltaTime;

			if(tmpPos.y > countPosition.position.y && transform.position.y <= countPosition.position.y) {
				MapController.Instance.AddCoins(nomination);
				Destroy(gameObject);
			}
		}
	}

	public void GoToPlayer() {
		player = Player.Jack.PlayerController.Instance.transform;
		toPlayer = true;
		StopAllCoroutines();
	}

	/// <summary>
	/// Установка значения номинации
	/// </summary>
	/// <param name="nom"></param>
	public void SetNomination(int nom) {
	
		nomination = nom;

		if(nom >= 2 && nom < 5)
			nom = 2;
		else if(nom >= 5 && nom < 10)
			nom = 5;
		else if(nom >= 10)
			nom = 10;

		graphAnim.SetTrigger(nom.ToString());
	}

	public void AddCouns() {

		if(OnGetPlayer != null) OnGetPlayer();
		OnGetPlayer = null;
		GameObject tc = Pooler.GetPooledObject("TakeCoin");
		tc.transform.position = transform.position;
		tc.SetActive(true);

		AudioManager.PlayEffect(audioClip[Random.Range(0, audioClip.Length)], AudioMixerTypes.runnerEffect);
		gameObject.SetActive(false);
		RunnerController.addRaceCoins(nomination);
	}

	public void GenMap(Transform positionCount) {
		countPosition = positionCount;
		velocity.y = Random.Range(1.5f, 2f);
		velocity.x = Random.Range(-0.5f, 0.5f);
		groundPointY = Random.Range(transform.position.y - 0.3f, transform.position.y + 0.3f);
		mapPhase = MapPhases.toCount;
	}

	public void OnTriggerPlayer(Player.Jack.PlayerController player) {
		AddCouns();
	}
}
