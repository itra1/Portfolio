using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Player.Interactions;

namespace it.Game.Managers
{
  /// <summary>
  /// Менеджер интеракционных обьектов при движении (исключающие работы с обьектами)
  /// </summary>
  public class InteractionsManager : MonoBehaviourBase
  {
	 private InteractionLibrary _library;
	 private List<InteractionMotion> _instancelibrary = new List<InteractionMotion>();

	 public InteractionLibrary Library
	 {
		get
		{
		  if (_library == null)
			 _library = Resources.Load<InteractionLibrary>(ProjectSettings.InteractionMotionsLibrary);
		  return _library;
		}
	 }

	 public InteractionMotion Get(InteractionMotion.MotionType type)
	 {
		InteractionMotion motion = _instancelibrary.Find(x=>x.Motion == type);

		if(motion == null)
		{
		  InteractionMotion prefab = Library.Get(type);
		  GameObject inst = InstantiateDefault(prefab.gameObject, transform);
		  motion = inst.GetComponent<InteractionMotion>();
		  _instancelibrary.Add(motion);
		}

		motion.gameObject.SetActive(true);
		return motion;
	 }

	 public void Leave(InteractionMotion motion)
	 {
		motion.gameObject.SetActive(false);
	 }


	 public enum InteractionType
	 {
		climbing_2_5
	 }

  }
}