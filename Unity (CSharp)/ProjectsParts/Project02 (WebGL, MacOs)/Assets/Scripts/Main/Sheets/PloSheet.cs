using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Main
{
	public class PloSheet : SheetBase
	{

		protected override void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.PLO_5_hi;
		}

	}
}