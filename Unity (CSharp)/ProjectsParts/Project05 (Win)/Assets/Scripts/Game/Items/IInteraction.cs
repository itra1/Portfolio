using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Items
{

  public interface IInteraction
  {
	 /// <summary>
	 /// Готов к использованию
	 /// </summary>
	 bool IsInteractReady { get; }

	 /// <summary>
	 /// Активация использования
	 /// </summary>
	 void StartInteract();
	 /// <summary>
	 /// Деактивация использования
	 /// </summary>
	 void StopInteract();


  }
}