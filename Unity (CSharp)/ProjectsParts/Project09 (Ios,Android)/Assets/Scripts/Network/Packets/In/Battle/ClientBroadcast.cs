using UnityEngine;
using System.Collections;

public class ClientBroadcast : Packet {

	public static event System.Action<string, string> OnBroadcast;

	string playerId;
	string text;

	public override void ReadImpl() {
		playerId = ReadASCII();
		text = ReadASCII();
	}

	public override void Process() {
		if(OnBroadcast != null) OnBroadcast(playerId, text);
	}

}
