using ExEvent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class BattleEventEffects : Singleton<BattleEventEffects> {

	public List<BattleEffectData> battleEffectData;             // Набор параметорв
	private List<BattleEffectShow> _battleEffectShowOrder = new List<BattleEffectShow>();       // Очередь для демонстрации

	private bool _firstKill = false;
	private bool _secondKill = false;
	private bool _thirdKill = false;
	[HideInInspector]
	public int _scaleCoeff = 0;

	private bool _isFirst = false;
	private bool _isStart;

	private Coroutine _showCoroutineInst;
	IEnumerator ShowDelay() {
		yield return new WaitForSeconds(0.1f);

		int damageCount = _battleEffectShowOrder.FindAll(x => x.instant.type == BattleEffectsType.killEnemy).Count;

		if (damageCount > 2) {

			CalcCoinsBonus();

			Vector3 showPos = _battleEffectShowOrder.FindLast(x => x.instant.type == BattleEffectsType.killEnemy).showPosition;
			if (damageCount >= 10) {
				_battleEffectShowOrder.Add(new BattleEffectShow() { showPosition = showPos, instant = battleEffectData.Find(x => x.type == BattleEffectsType.enemy10Kill) });
			} else if (damageCount >= 6) {
				_battleEffectShowOrder.Add(new BattleEffectShow() { showPosition = showPos, instant = battleEffectData.Find(x => x.type == BattleEffectsType.enemy6_9Kill) });
			} else
				_battleEffectShowOrder.Add(new BattleEffectShow() { showPosition = showPos, instant = battleEffectData.Find(x => x.type == BattleEffectsType.enemy3_5Kill) });
		}

		_battleEffectShowOrder.RemoveAll(x => x.instant.type == BattleEffectsType.killEnemy);

		List<BattleEffectShow> readyShow = _battleEffectShowOrder.OrderByDescending(x => x.instant.priority).ToList();

		List<PositionType> showPosition = new List<PositionType>();

		foreach (var readyShowElement in readyShow) {
			if (!readyShowElement.instant.allEffects && showPosition.Contains(readyShowElement.instant.position)) continue;
			showPosition.Add(readyShowElement.instant.position);
			readyShowElement.instant.Show(readyShowElement.showPosition);
		}
		_battleEffectShowOrder.Clear();

	}

	void CalcCoinsBonus() {
		List<BattleEffectShow> killList = _battleEffectShowOrder.FindAll(x => x.instant.type == BattleEffectsType.killEnemy);

		int summCoins = 0;

		foreach (var killElement in killList) {
			try {
				summCoins += EnemysSpawn.Instance.CalcSilverCoins(killElement.targetObject as Enemy,
					(killElement.targetObject as Enemy).lastDamage);
			} catch { }
		}

		int resultCoins = (int)(0.5f * summCoins);

		if (killList.Count >= 10) {
			resultCoins = (int)(2f * summCoins);
		} else if (killList.Count >= 6) {
			resultCoins = (int)(1f * summCoins);
		}

		BattleManager.Instance.silverCoins += resultCoins;
	}

	[ExEvent.ExEventHandler(typeof(BattleEvents.StartBattle))]
	void BattleStart(BattleEvents.StartBattle startEvent) {
		_isStart = true;
		_firstKill = false;
		_secondKill = false;
		_thirdKill = false;
		_isFirst = true;
	}

	public void VisualEffect(BattleEffectsType type, Vector3 showPosition, params object[] paramsArr) {

		if (!_isStart) return;

		showPosition += Vector3.up * 2;

		switch (type) {
			case BattleEffectsType.killEnemy: {
					BattleEffectShow instElem = ShowEffect(type, showPosition);
					if (instElem != null && type == BattleEffectsType.killEnemy)
						instElem.targetObject = paramsArr[0];

					if (_isFirst) {
						
						BonusShow bs = new BonusShow();
						bs.showPosition = showPosition;
						
						if (!_firstKill) {
							type = BattleEffectsType.enemyFirstKill;
							_firstKill = true;
							bs.coinsSize = 20;
							BattleManager.Instance.silverCoins += 20;
							bonusShowQueue.Enqueue(bs);
							StartshowBonus();
						} else if (!_secondKill) {
							type = BattleEffectsType.enemySecondKill;
							_secondKill = true;
							bs.coinsSize = 30;
							BattleManager.Instance.silverCoins += 30;
							bonusShowQueue.Enqueue(bs);
							StartshowBonus();
						} else if (!_thirdKill) {
							type = BattleEffectsType.enemyTristedKill;
							_thirdKill = true;
							bs.coinsSize = 50;
							BattleManager.Instance.silverCoins += 50;
							bonusShowQueue.Enqueue(bs);
							StartshowBonus();
						} else {
							return;
						}

					}
					
					break;
				}
			case BattleEffectsType.enyHitAfter5:
				_scaleCoeff = paramsArr.Length > 0 ? (int)paramsArr[0] : 10;

				switch (_scaleCoeff) {
					case 10:
						BonusShow bs1 = new BonusShow();
						bs1.showPosition = showPosition;
						bs1.coinsSize = 10;
						BattleManager.Instance.silverCoins += 10;
						bonusShowQueue.Enqueue(bs1);
						StartshowBonus();
						break;
					case 20:
					BonusShow bs2 = new BonusShow();
						bs2.showPosition = showPosition;
						bs2.coinsSize = 20;
						BattleManager.Instance.silverCoins += 20;
						bonusShowQueue.Enqueue(bs2);
						StartshowBonus();
						break;
					case 40:
						BonusShow bs3 = new BonusShow();
						bs3.showPosition = showPosition;
						bs3.coinsSize = 30;
						BattleManager.Instance.silverCoins += 30;
						bonusShowQueue.Enqueue(bs3);
						StartshowBonus();
						break;
					case 80:
						BonusShow bs4 = new BonusShow();
						bs4.showPosition = showPosition;
						bs4.coinsSize = 40;
						BattleManager.Instance.silverCoins += 40;
						bonusShowQueue.Enqueue(bs4);
						StartshowBonus();
						break;
					case 160:
						BonusShow bs5 = new BonusShow();
						bs5.showPosition = showPosition;
						bs5.coinsSize = 50;
						BattleManager.Instance.silverCoins += 50;
						bonusShowQueue.Enqueue(bs5);
						StartshowBonus();
						break;
				}
				
				break;
			default:
				break;
		}

		ShowEffect(type, showPosition);

	}

	BattleEffectShow ShowEffect(BattleEffectsType type, Vector3 showPosition) {
		BattleEffectData show = battleEffectData.Find(x => x.type == type);

		if (show == null) {
			Debug.LogError(String.Format("Отсутствует эффект с типом {0}", type));
			return null;
		}

		if (type != BattleEffectsType.killEnemy && show.prefab == null) {
			Debug.LogError(String.Format("Эффекту с типом {0} не назначен префаб ", type));
			return null;
		}

		BattleEffectShow newItem = new BattleEffectShow() { showPosition = showPosition, instant = show };

		_battleEffectShowOrder.Add(newItem);

		if (_showCoroutineInst != null) StopCoroutine(_showCoroutineInst);
		_showCoroutineInst = StartCoroutine(ShowDelay());

		return newItem;
	}

	[HideInInspector]
	public int _hitCount = 0;
	private Enemy _lastEnemy = null;

	public GameObject bonusPrefab;
	private List<GameObject> _bonusPrefabInstance = new List<GameObject>();       // Очередь для демонстрации

	public void DestroyBullet(Game.Weapon.WeaponType wt, Enemy enemy, Vector3 pos) {

		if (enemy != null && (_lastEnemy == null || _lastEnemy == enemy) && enemy.transform.position.x >= -7) {
			_hitCount++;
		} else {
			_isFirst = false;
			_hitCount = 0;
		}
		if (_hitCount >= 5) {
			if (wt != Game.Weapon.WeaponType.tomato || wt != Game.Weapon.WeaponType.axe || wt != Game.Weapon.WeaponType.brick || wt != Game.Weapon.WeaponType.radiator)
				BattleManager.Instance.silverCoins ++;
			VisualEffect(BattleEffectsType.enyHitAfter5, pos + Vector3.down * 2, _hitCount);
		}
	}

	[HideInInspector]
	public Queue<BonusShow> bonusShowQueue = new Queue<BonusShow>();

	private Coroutine _bonusShowCor;
	public IEnumerator ShowBonus() {
		yield return new WaitForSeconds(0.1f);
		while (bonusShowQueue.Count > 0) {
			BonusShow bs = bonusShowQueue.Dequeue();

			GameObject pr = _bonusPrefabInstance.Find(x => !x.activeInHierarchy);

			if (pr == null) {
				pr = Instantiate(bonusPrefab);
				_bonusPrefabInstance.Add(pr);
			}

			pr.transform.position = bs.showPosition + Vector3.up*1.5f;
			pr.GetComponent<BonusEffect>().SetValue(bs.coinsSize);
			// Устанавливаем сумму
			pr.SetActive(true);
			yield return new WaitForSeconds(1f);
		}
	}

	void StartshowBonus() {
		if (_bonusShowCor != null) StopCoroutine(_bonusShowCor);
		_bonusShowCor = StartCoroutine(ShowBonus());
	}

}

/// <summary>
/// Тип позиционирования
/// </summary>
public enum PositionType {
	custom,                     // Кастомный эффект
	center,                     // Центер экрана, при этом игнорируетсяточка генерации
	player                      // Позиционирование над плеером
}

public enum BattleEffectsType {
	none
	, lastEnemy                     // Последний враг на сцене
	, enemy10Kill                   // Убито 10 за раз
	, enemy6_9Kill                    // Убито 6-9 за раз
	, enemy3_5Kill                    // Убито 3-5 за раз
	, enemyLastKill                 // Убит последний моб
	, enemyFirstKill                  // Первый убитый моб
	, enemySecondKill               // Второй убитый моб
	, enemyTristedKill                // Третий убитый моб
	, enyHitAfter5                    // Точное попадание с каждого 5го
	, levelUp                       // Повышение уровня
	, loading                       // Загрузка
	, loose                         // Пройгрыш
	, killTusModHip                 // Убийство тусилы, модника хипстуры
	, bazookaBom                      // Попадание базукой
	, granadeHit                      // Попадание гранатой по первому игроку
	, lowEnergy                     // Мало энергии
	, hunterObrezShoot                // Над всеми от охотника и дробовика
	, molotov5                        // При уроне коктейлем молотова от 5 и выше
	, win                           // Успешное окончание боя
	, iskanderHit                   // Успешный урон искандером
	, bossKill                        // Убийство босса
	, killEnemy                     // Убийство врага
}

[System.Serializable]
public class BattleEffectData {
	public string title;
	public BattleEffectsType type;
	public PositionType position;
	public int priority;
	public bool allEffects;
	public bool noDouble;
	public float koeffScale = 1;

	public GameObject prefab;

	private List<GameObject> _instanceList = new List<GameObject>();

	public void Show(Vector3 position) {

		GameObject inst = _instanceList.Find(x => !x.activeInHierarchy);

		if (noDouble && inst == null && _instanceList.Count > 0) return;

		if (inst == null) {
			inst = MonoBehaviour.Instantiate(prefab);
			_instanceList.Add(inst);
		}

		switch (this.position) {
			case PositionType.custom:
				inst.transform.position = position;
				break;
			case PositionType.player:
				inst.transform.position = PlayerController.Instance.transform.position;
				break;
			case PositionType.center:
			default:
				inst.transform.position = Vector3.zero;
				break;
		}

		if (type == BattleEffectsType.enyHitAfter5) {
			float scale = 0.3f;
			for (int i = 0; i < (BattleEventEffects.Instance._scaleCoeff-5); i++) scale *= 1.05f;

			if (scale > 1) scale = 1;

			inst.transform.localScale = new Vector3(scale, scale, scale);
			
		}

		inst.SetActive(true);
	}

	public bool IsShow() {
		GameObject inst = _instanceList.Find(x => x.activeInHierarchy);

		return inst != null;
	}

}

[System.Serializable]
public class BattleEffectShow {
	public Vector3 showPosition;
	public BattleEffectData instant;
	[HideInInspector]
	public object targetObject;
}

public class BonusShow {
	public Vector3 showPosition;
	public int coinsSize;
}