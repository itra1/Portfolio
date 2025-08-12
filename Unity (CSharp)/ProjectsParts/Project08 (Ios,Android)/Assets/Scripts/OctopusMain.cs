using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using Spine.Unity;

public class OctopusMain : Singleton<OctopusMain>{

	private OctopusSpine _spine;
	public ParticleSystem salut;
	
	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnAddDecor))]
	public void ChengeAchive(ExEvent.PlayerEvents.OnAddDecor decor) {
		salut.Play();
		int num = Random.Range(0, 3);
		switch (num) {
			case 0:
				_spine.PlayAnim(OctopusSpine.yesAnim, false);
				break;
			case 1:
				_spine.PlayAnim(OctopusSpine.supriseAnim, false);
				break;
			case 2:
				_spine.PlayAnim(OctopusSpine.happyAnim, false);
				break;
		}
	}

	[ExEvent.ExEventHandler(typeof(GameEvents.OnFishTouch))]
	private void OnFishTouch(GameEvents.OnFishTouch fish) {

		return;

		if (Random.value > 0.3f) return;

		if(_spine.actualAnimation == OctopusSpine.idleAnim)
			_spine.PlayAnim(Random.value > 0.5f ? OctopusSpine.happyAnim : OctopusSpine.yesAnim, false);

	}

	public void PlayWondering() {
		int num = Random.Range(0, 4);
		switch (num) {
			case 0:
				_spine.PlayAnim(OctopusSpine.wonderingAnim, false);
				break;
			case 1:
				_spine.PlayAnim(OctopusSpine.wondering2Anim, false);
				break;
			case 2:
				_spine.PlayAnim(OctopusSpine.wondering3Anim, false);
				break;
			case 3:
				_spine.PlayAnim(OctopusSpine.wondering4Anim, false);
				break;
		}
	}
	

	private void OnEnable() {
		_spine = GetComponent<OctopusSpine>();
		_spine.OnCompleted = (track)=>
		{

			if (track == OctopusSpine.idleAnim) {
				idleCount++;
				if (idleCount == 6) {
					idleCount = 0;
					ClickRandomAnimation();
				}
			}

			if (track != OctopusSpine.idleAnim)
				_spine.PlayAnim(OctopusSpine.idleAnim, true);

		};

		_spine.OnRebuild = () => {
			_spine.PlayAnim(OctopusSpine.idleAnim, true);
		};

	}

	public void Click() {
		ClickRandomAnimation();
	}

	private void ClickRandomAnimation() {
		if (_spine.actualAnimation != OctopusSpine.idleAnim) return;
		int num = Random.Range(0, 12);
		switch (num) {
			case 0:
				_spine.PlayAnim(OctopusSpine.curiousAnim, false);
				break;
			case 1:
				_spine.PlayAnim(OctopusSpine.despondencyAnim, false);
				break;
			case 2:
				_spine.PlayAnim(OctopusSpine.disgustinglyAnim, false);
				break;
			case 3:
				_spine.PlayAnim(OctopusSpine.disgustingly2Anim, false);
				break;
			case 4:
				_spine.PlayAnim(OctopusSpine.reactAnim, false);
				break;
			case 5:
				_spine.PlayAnim(OctopusSpine.singAnim, false);
				break;
			case 6:
				_spine.PlayAnim(OctopusSpine.sing2Anim, false);
				break;
			case 7:
				_spine.PlayAnim(OctopusSpine.sing3Anim, false);
				break;
			case 8:
				_spine.PlayAnim(OctopusSpine.wonderingAnim, false);
				break;
			case 9:
				_spine.PlayAnim(OctopusSpine.wondering2Anim, false);
				break;
			case 10:
				_spine.PlayAnim(OctopusSpine.wondering3Anim, false);
				break;
			case 11:
				_spine.PlayAnim(OctopusSpine.wondering4Anim, false);
				break;
		}
	}

	private int idleCount = 0;
	
}
