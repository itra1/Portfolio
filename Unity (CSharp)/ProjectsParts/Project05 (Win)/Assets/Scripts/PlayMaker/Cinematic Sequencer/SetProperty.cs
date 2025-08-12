using UnityEngine;
using HutongGames.PlayMaker;
using Slate;

namespace it.Game.PlayMaker.Cinematic
{
  [ActionCategory("Cinematic")]
  public class SetPropertyBase : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _cutScene;
	 [RequiredField]
	 public FsmString _groupName;
	 [RequiredField]
	 public FsmString _trackName;
	 [RequiredField]
	 public FsmGameObject _gameObject;

	 protected CutsceneGroup _group;
	 protected CutsceneTrack _track;

	 public override void OnEnter()
	 {
		Cutscene cutscene = _cutScene.Value.GetComponent<Cutscene>();
		
		for (int i = 0; i < cutscene.groups.Count; i++)
		{
		  if (cutscene.groups[i].name.Equals(_groupName.Value))
		  {
			 _group = cutscene.groups[i]; 
			 for (int t = 0; t < _group.tracks.Count; t++)
			 {
				if (cutscene.groups[i].tracks[t].name.Equals(_trackName.Value))
				{
				  _track = cutscene.groups[i].tracks[t];
				  for(int c = 0; c < _track.clips.Count; c++)
				  {
					 Debug.Log(_track.clips[c] is Slate.ActionClips.MatchTransformsToTarget);
				  }
				}
			 };
		  }
		};

	 }
  }
}