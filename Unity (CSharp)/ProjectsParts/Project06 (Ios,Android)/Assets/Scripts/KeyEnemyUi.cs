using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyEnemyUi : MonoBehaviour {

	public Animation animationComponent;

	public Image icon;

	private EnemyType _enemyType;

	public RectTransform bloodTransform;

	public GameObject skull;
	public GameObject blackMask;

	private Enemy useEnemy;

	private bool isDead = true;

	public void SetIcon() {
		EnemyInfo ei = GameDesign.Instance.enemyInfo.Find(x => x.type == _enemyType);
		icon.sprite = ei.icon;
	}

	public void Show(EnemyType enemyType) {
		
		_enemyType = enemyType;
		animationComponent.Play("keyEnemyShow");
		blackMask.SetActive(true);
		skull.SetActive(false);
		bloodTransform.anchoredPosition = Vector2.zero;
		SetIcon();

		if (useEnemy != null) {
			useEnemy.OnDead -= DeadEnemy;
			useEnemy.OnDamageEvnt -= DamageEnemy;
		}

	}

	public void Hide() {
		//animationComponent.Play("keyEnemyHide");
	}

	public void GenerateKeyEnemy(Enemy enemy) {
		useEnemy = enemy;

		useEnemy.OnDead += DeadEnemy;
		useEnemy.OnDamageEvnt += DamageEnemy;

		blackMask.SetActive(false);
		bloodTransform.anchoredPosition = Vector2.zero;
		isDead = false;

	}

	private void DeadEnemy(Enemy enemy) {
		useEnemy.OnDead -= DeadEnemy;
		useEnemy.OnDamageEvnt -= DamageEnemy;
		blackMask.SetActive(true);
		skull.SetActive(true);
		bloodTransform.anchoredPosition = Vector2.zero;
		isDead = true;
	}
	private void DamageEnemy(Enemy enemy, GameObject source, float damage) {
		if (isDead) return;

		bloodTransform.anchoredPosition = new Vector2(0,Mathf.Lerp(-77,0,((float)(enemy.liveNow- damage) / (float)enemy.startLive)));
		
	}

	public void Change(EnemyType enemyType) {
		this._enemyType = enemyType;
		animationComponent.Play("keyEnemyChange");
	}

	public void Close() {
		gameObject.SetActive(false);
	}

}
