using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties : Singleton<Properties> {

	public string moreGames {
		get {
#if UNITY_IOS
			return iosMoreGame;
#elif UNITY_ANDROID
			return androidMoreGames;
#else
			return "";
#endif
		}
	}

	public string iosMoreGame = "1013592798";
	public string androidMoreGames = "5415301411857753051";


	public int hintFirstLetterPrice = 25;
	public int hintAnyLetterPrice = 40;
	public int hintFirstWordPrice = 100;

}
