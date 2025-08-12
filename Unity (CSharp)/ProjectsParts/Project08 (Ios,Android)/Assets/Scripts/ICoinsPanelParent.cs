using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICoinsPanelParent {

	Canvas coinsPanel { get; }
	RectTransform coinsTargetIcon { get; }
	CoinsWidget coinsWidget { get; }

	void CoinsPanelGiftOpen();

}
