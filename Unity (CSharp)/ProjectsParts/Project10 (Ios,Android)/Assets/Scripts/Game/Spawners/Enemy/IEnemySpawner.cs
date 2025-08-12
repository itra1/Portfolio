using UnityEngine;
using System.Collections;

public interface IEnemySpawner {
	/// <summary>
	/// Инифиализация
	/// </summary>
	/// <param name="spawner"></param>
	void Init(EnemySpawner spawner);

	/// <summary>
	/// Отключение
	/// </summary>
	void DeInit();

	/// <summary>
	/// Ежекажровый выхов
	/// </summary>
	void Update();

	void OnSpecialPlatform(ExEvent.RunEvents.SpecialPlatform specialPlatform);
	void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier specialBarrier);

	void DeadCloud();
}
