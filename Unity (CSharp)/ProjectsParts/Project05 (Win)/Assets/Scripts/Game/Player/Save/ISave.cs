using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Player.Save
{
  /// <summary>
  /// Сохранение
  /// </summary>
  public interface ISave
  {
	 void SubscribeSaveEvents();

	 void UnsubscribeSaveEvents();

	 void LoadHandler(com.ootii.Messages.IMessage handler);

	 void Load(Game.Player.Save.PlayerProgress progress);

	 void Save();

  }
}