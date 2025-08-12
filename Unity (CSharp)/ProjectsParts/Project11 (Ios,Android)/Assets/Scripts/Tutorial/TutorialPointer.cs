using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

public class TutorialPointer : Singleton<TutorialPointer> {

	public TutorialMover pointer;

	private List<AlphaFloatBehaviour> _alphaList;
	private List<GameWord> _wordList;

	private bool readyNext = true;

	private void OnEnable() {
		pointer.OnMoveComplete = OnMoveComplete;
	}

	private void OnMoveComplete() {

		if (!readyNext) return;

		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		_wordList = _wordList = new List<GameWord>(word.gameCrossword.wordList).FindAll(x => !x.isOpen);

		if (_wordList.Count == 0) return;

		NextMover();

	}

	private void NextMover() {

		List<AlphaFloatBehaviour> readyList = new List<AlphaFloatBehaviour>();
		
		bool isAdded = false;

		for (int i = 0; i < _wordList[0].word.word.Length; i++) {
			isAdded = false;
			for (int j = 0; j < _alphaList.Count; j++) {
				if (!isAdded && _wordList[0].word.word[i].ToString().ToLower() == _alphaList[j].alpha.ToLower() && !readyList.Contains(_alphaList[j])) {
					readyList.Add(_alphaList[j]);
					isAdded = true;
				}
			}

		}
		
		List<Vector3> positionsList = new List<Vector3>();
		readyList.ForEach(x => positionsList.Add(x.transform.position));
		
		pointer.Play(positionsList);
	}

	[ExEvent.ExEventHandler(typeof(TutorialEvents.PlayMovePointer))]
	private void PlayMovePointer(TutorialEvents.PlayMovePointer level) {
    if (Tutorial.Instance.isTutorial) {
      pointer.gameObject.SetActive(true);
      PlayMove();
    } else {
      pointer.gameObject.SetActive(false);
    }
	}

	private void PlayMove() {

		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		_alphaList = new List<AlphaFloatBehaviour>(word.lettersController.alphaList);
		_wordList = new List<GameWord>(word.gameCrossword.wordList);

		NextMover();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
	public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

		if (selectWord.select == SelectWord.yes) {
			pointer.Stop();
			readyNext = true;
		}
	}
	
	[ExEvent.ExEventHandler(typeof(TutorialEvents.TutorialEnd))]
	private void OnLevelLoad(TutorialEvents.TutorialEnd level) {
		gameObject.SetActive(false);
	}

}
