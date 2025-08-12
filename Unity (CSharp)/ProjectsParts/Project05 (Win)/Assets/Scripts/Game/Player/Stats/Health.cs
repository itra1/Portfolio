using UnityEngine;
using System.Collections;

namespace it.Game.Player.Stats
{
  /// <summary>
  /// Значение жизней
  /// </summary>
  public class Health : MonoBehaviourBase
  {
	 /// <summary>
	 /// Коменда о смести игрока
	 /// </summary>
	 public const string EVT_IS_DEAD = "PLAYER_IS_DEAD";
	 /// <summary>
	 /// Коменда о смести игрока
	 /// </summary>
	 public const string EVT_HEALTH_CHANGE = "PLAYER_HEALTH_CHANGE";
	 /// <summary>
	 /// МАксимальное значение жизней
	 /// </summary>
	 public float MaxValue => _maxValue;
	 /// <summary>
	 /// Текущий показатель жизней
	 /// </summary>
	 public float Value { get => _value; set => _value = value; }
	 /// <summary>
	 /// Значение жизней в процентном соотношении
	 /// </summary>
	 public float HealthPercent => Value / MaxValue;

	 private float _maxValue = 100;
	 private float _value = 0;

	 /// <summary>
	 /// Время протекции между получениями урона
	 /// </summary>
	 private float _gracePeriod = 2;
	 /// <summary>
	 /// Время последнего получения урона
	 /// </summary>
	 private float _lastDamageTime = 0;
	 /// <summary>
	 /// Скорость восстановления единиц в секунду
	 /// </summary>
	 private float _RecoverySpeed = 1;

	 /// <summary>
	 /// Разрешение на восстановление
	 /// </summary>
	 private bool _isRecovery = false;

	 /// <summary>
	 /// Смерть
	 /// </summary>
	 public bool IsDead => Value <= 0;

	 /// <summary>
	 /// Полные жизни
	 /// </summary>
	 public bool IsFull => Value >= MaxValue;

	 /// <summary>
	 /// Инициализация
	 /// </summary>
	 public void Init()
	 {
		Restore();
	 }


	 private void Update()
	 {
		Rcovery();
	 }

	 /// <summary>
	 /// Ресторе значения жизней
	 /// </summary>
	 public void Restore()
	 {
		Value = MaxValue;
		_lastDamageTime = 0;
		SendMessaheHealthChange();
	 }

	 /// <summary>
	 /// Изменение значения
	 /// </summary>
	 /// <param name="value"></param>
	 public void Add(float value)
	 {

		if (IsDead || IsFull)
		  return;

		this.Value +=  value;
		this.Value = Mathf.Clamp(this.Value, 0, MaxValue);

		SendMessaheHealthChange();
	 }

	 public bool Damage(Component obj)
	 {
		bool isDead = false;
		return Damage(obj, 100, ref isDead, true);
	 }

	 /// <summary>
	 /// Получение урона
	 /// </summary>
	 /// <param name="value">РАзмер получаемого урона</param>
	 /// <param name="ignoreProtectPerion">Игнорировать период ожидания</param>
	 public bool Damage(Component obj, float value, bool ignoreProtectPerion = false)
	 {
		bool isDead = false;
		return Damage(obj,value, ref isDead, ignoreProtectPerion);

	 }
	 public bool Damage(Component obj, float value, ref bool isDead, bool ignoreProtectPerion = false)
	 {
		if ((ignoreProtectPerion && _lastDamageTime + 0.01f > Time.time) || (_lastDamageTime + _gracePeriod > Time.time))
		  return false;


		if (Configs.IsGood) 
		  return false;

#if UNITY_EDITOR
		Debug.Log(string.Format("Damage {0} {1}", obj.gameObject.name, value));

		var parent = obj.transform.parent;
		while (parent != null)
		{
		  Debug.Log(string.Format("Parent {0}", parent.gameObject.name));
		  parent = parent.transform.parent;
		}

#endif

		if (IsDead)
		  return false;

		_lastDamageTime = Time.time;

		this.Value -= value;
		this.Value = Mathf.Clamp(this.Value, 0, MaxValue);

		SendMessaheHealthChange();

		isDead = ChechDead();

		return true;

	 }

	 public void Damage(Component obj, Game.Player.Handlers.IPlayerDamage damageHandler)
	 {

		bool isDead = false;
		var isDamage = Damage(obj,damageHandler.PlayerDamageValue, ref isDead, damageHandler.IgnoreGracePeriod);
	 }

	 private void SendMessaheHealthChange()
	 {
		Game.Events.EventDispatcher.SendMessage(EVT_HEALTH_CHANGE);
	 }
	 private void SendMessaheIsDead()
	 {
		Game.Events.EventDispatcher.SendMessage(EVT_IS_DEAD);
	 }

	 /// <summary>
	 /// Проврка на смерть
	 /// </summary>
	 private bool ChechDead()
	 {
		if (this.Value > 0)
		  return false;

		SendMessaheIsDead();
		return true;

	 }


	 private void Rcovery()
	 {
		if (!_isRecovery || IsFull || IsDead)
		  return;

		Value = Mathf.Clamp(Value, Value + _RecoverySpeed * Time.deltaTime, MaxValue);

	 }


  }
}