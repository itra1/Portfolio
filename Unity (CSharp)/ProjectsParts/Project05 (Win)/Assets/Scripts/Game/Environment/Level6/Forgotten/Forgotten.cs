using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes;

namespace it.Game.Environment.Level6.Forgotten
{
  /// <summary>
  /// 
  /// 
  /// Забытый. Форма и Паззл
  //1. Увеличить забытого в два раза
  //2. Построить вокруг него 5 ярусную конструкцию из колонн и полукруглых
  //платформ.На равном удалении друг от друга разместить башни(с 3уровня) с
  //техногенными обелисками(башни здания с 7 уровня) на верхушках.
  //3. В Башни должно быть два входа и выхода.
  //4. У башен три этажа
  //5. На втором этаже находятся огненные големы которые кидают в персонажа
  //огненными шарами(анимация обычного удара) по примеру мимиков.
  //6. Големы закреплены на постаментах и не могут ходить на могут стрелять на
  //расстояние до 15-20 метров.То есть они могут стрелять и по серпантинам.
  //7. Големы всех трех башен активируются как только персонаж заходит в первую
  //башню и отключаются после прохождения задания.
  //8. Цель включить 3 обелиска чтобы накормить забытого
  //9. Три обелиска формируют лучами сферу над забытым.Как только все три
  //заработают из сферы начнет течь энергия ему в рот а его глаза начнут
  //светиться желтым.Персонажа телепортирует напротив забытого за пределы
  //руин
  //10. (по возможности) Забытый издает крик разрушая руины
  //11. После этого из его рта вылетает кристалл
  /// 
  /// </summary>

  public class Forgotten : Environment
  {
	 /* Состония
	  *  0 - стартовое состояние
	  * 
	  */

	 /// <summary>
	 /// Список обелисков
	 /// </summary>
	 private ForgottenObelisk[] _obelisks;
	 private ForgottenMob _mob;
	 private ShooterStaticFireGolem[] _golems;

	 private bool _golemsActive;

	 protected override void Awake()
	 {
		base.Awake();
		_obelisks = GetComponentsInChildren<ForgottenObelisk>();
		_mob = GetComponentInChildren<ForgottenMob>();
		_golems = GetComponentsInChildren<ShooterStaticFireGolem>();
		Subscribe();
		Clear();
	 }

	 private void Subscribe()
	 {
		for(int i = 0; i < _obelisks.Length; i++)
		{
		  _obelisks[i].OnActivate = ObeiskActivate;
		}
	 }

	 private void ObeiskActivate()
	 {
		bool allactivate = true;

		for (int i = 0; i < _obelisks.Length; i++)
		{
		  if (!_obelisks[i].IsActivate)
			 allactivate = false;
		}

		if (allactivate)
		{
		  _mob.EyeActivate();
		}

	 }

	 /// <summary>
	 /// Очистка состояния
	 /// </summary>
	 private void Clear()
	 {
		State = 0;

		for (int i = 0; i < _obelisks.Length; i++)
		  _obelisks[i].Clear();
		for(int i = 0; i < _golems.Length; i++)
		{
		  PlayMakerFSM fsm = _golems[i].GetComponent<PlayMakerFSM>();
		  fsm.SendEvent("OnDeactive");
		  _golems[i].Deactivate();
		}
		_golemsActive = false;
		_mob.Clear();
	 }

	 private void ActivateGolems()
	 {
		for (int i = 0; i < _golems.Length; i++)
		{
		  PlayMakerFSM fsm = _golems[i].GetComponent<PlayMakerFSM>();
		  fsm.SendEvent("OnDeactive");
		}
	 }

	 /// <summary>
	 /// Вхождение в башню
	 /// </summary>
	 public void TowerTrigger()
	 {
		if (_golemsActive) return;

		for (int i = 0; i < _golems.Length; i++)
		{
		  PlayMakerFSM fsm = _golems[i].GetComponent<PlayMakerFSM>();
		  fsm.SendEvent("OnActivate");
		}
	 }

	 /// <summary>
	 /// Вход в зону забытого
	 /// </summary>
	 private void PlayerEnter()
	 {
		ActivateGolems();
	 }

  }
}