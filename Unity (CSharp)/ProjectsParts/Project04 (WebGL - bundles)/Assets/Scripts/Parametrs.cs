using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parametrs : Singleton<Parametrs> {
	
	private string _api = "www.romewar.ru";

	private string _testBundle = "http://files.netarchitect.ru/bundle/";
	private string _bundle = "https://www.romewar.ru/game/battles/unity/";


	public string apiServer {
		get {
#if UNITY_EDITOR
			return "http://" + _api;
#else
			return "https://" + _api;
#endif
		}
	}

	public string bundleServer {
		get {
#if UNITY_EDITOR
			return _testBundle;
#else
			return _bundle;
#endif
		}
	}


}
