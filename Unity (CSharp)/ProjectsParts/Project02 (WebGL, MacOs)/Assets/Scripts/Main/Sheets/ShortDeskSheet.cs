using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Main
{
	public class ShortDeskSheet : SheetBase
	{
		protected override void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.ShortDesk;
		}
	}
}