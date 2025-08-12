using UnityEngine;
using HutongGames.PlayMaker;
using Slate;

namespace it.Game.PlayMaker.Cinematic
{
	[ActionCategory("Cinematic")]
	public class SetActor : FsmStateAction
	{
		[RequiredField]	public FsmGameObject _cutScene;
		[RequiredField]	public FsmString _groupName;
		[RequiredField]	public FsmGameObject _gameObject;

		public override void OnEnter()
		{
			Cutscene cutscene = _cutScene.Value.GetComponent<Cutscene>();
			//cutscene.groups[0].tracks[0].
			for (int i = 0; i < cutscene.groups.Count; i++)
			{
				if (cutscene.groups[i].name.Equals(_groupName.Value))
					cutscene.groups[i].actor = _gameObject.Value;
			};

		}
	}
}