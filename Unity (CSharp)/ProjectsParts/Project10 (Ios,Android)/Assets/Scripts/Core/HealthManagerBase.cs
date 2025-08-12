using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип жизней
/// </summary>
public enum HealthType {
  hearh,                    // Сердца							
  power,                    // Сила, используется на корабле
  energy                    // Энергия, используется как замена сердец
}

/// <summary>
/// Интерфейс менеджера жизней
/// </summary>
public abstract class HealthManagerBase : MonoBehaviour {

  /// <summary>
  /// Событие изменения значения жизней
  /// </summary>
  /// <para>Актуальное значение жизней</para>
  /// <para>Максимальное значение жизней</para>
  public static event Action<float, float> OnChangeEvent;
	
	public HealthType type;
	public bool useLivePerk;
	[SerializeField]
	protected float _maxValue;
	public float maxValue { get; protected set; }

	[SerializeField]
	protected float livePerkValue;

	public float actualValue { get; protected set; }

	private void Start() {
		Init();
	}

	public abstract void Init();

	public abstract void LiveChange(float delta);

	protected void ChangeEvent() {
		if (OnChangeEvent != null) OnChangeEvent(actualValue, maxValue);
	}

}
