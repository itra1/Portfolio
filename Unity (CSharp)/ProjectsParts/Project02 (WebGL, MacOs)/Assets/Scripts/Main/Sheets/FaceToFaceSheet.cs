using it.Main;
using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Main
{
	public class FaceToFaceSheet : SheetBase
	{
		protected override void GetType(Table rec)
		{
			rec.PokerGameType = PokerGameType.OFC;
		}
	}
}