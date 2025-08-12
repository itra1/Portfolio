using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniMenuParent {

	GameObject gameMiniMenu { get; }

	GameMiniMenu miniMenu { get; }

	void MiniMenyGiftOpen(bool isLocked);
	void MiniMenyGiftClose(bool isLocked);

}
