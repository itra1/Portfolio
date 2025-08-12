using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ZbCatScene.CatSceneManager))]
public class CatSceneManagerEditor : Editor {
	private string _id;
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal();
		_id = EditorGUILayout.TextField("Идентификатор", _id);

		if (GUILayout.Button("Show"))
			((ZbCatScene.CatSceneManager) target).ShowCatScene(_id, null, true);

		GUILayout.EndHorizontal();

	}
}

#endif

namespace ZbCatScene {

	public class CatSceneManager : Singleton<CatSceneManager>
	{

    /// <summary>
    /// Отключение кат сцен
    /// </summary>
	  //private bool _isOn = false;

	  public bool IsOn { get; private set; } = false;

    public Action OnComplited;

		public CatSceneLibrary library;
		
		public  CatScene _activeCatScene;
		private CatBlockBehaviour _activeBlock;
		private string _id;
		private int _step;
		private CatSceneUi _catSceneUi;

		public bool isSpecLevel;
		
		public static bool isActive { get; set; }
    
    public bool ShowCatScene(int num, Action OnComplited = null, bool isTest = false)
    {
      
			return ShowCatScene(num.ToString(), OnComplited, isTest);
		}

		public bool ShowCatScene(string num, Action OnComplited = null, bool isTest = false) {

		  if (!IsOn) {
		    if (OnComplited != null)
		      OnComplited();
		    return false;
		  }

      CatScene checkScene = library.catSceneList.Find(x => x.id == num);

			if(!isTest && checkScene.isShow) return false;
			_activeCatScene = library.catSceneList.Find(x => x.id == num);

			if (_activeCatScene.isPause) Time.timeScale = 0;

			isActive = true;
			this.OnComplited = OnComplited;
			this._id = num;
			ExEvent.CatSceneEvent.StartCatScene.Call(num);

			_step = -1;

			if (_catSceneUi == null)
				_catSceneUi = UIController.ShowUi<CatSceneUi>();
			_catSceneUi.gameObject.SetActive(true);

			_catSceneUi.OnNext = NextScene;

			NextScene();

			return true;
		}

		public bool CheckShowCatScene(int num) {
			return CheckShowCatScene(num.ToString());
		}
		public bool CheckShowCatScene(string num) {
			CatScene checkScene = library.catSceneList.Find(x => x.id == num);
			return checkScene.isShow;
		}

		public void NextScene(bool isHelper = false) {

			_catSceneUi.gameObject.SetActive(false);
			ExEvent.CatSceneEvent.EndCatFrame.Call(_id, _step);

			if (_step + 1 >= _activeCatScene.catBlockList.Count) {
				EndCatScene();
				return;
			}
			

			if(!isHelper) ExEvent.CatSceneEvent.StartCatFrame.Call(_id, _step+1);
			if (!isHelper && _activeCatScene.catBlockList[_step + 1].helperStart) return;

			NextStep();
		}

		private Coroutine _soundCor;
		IEnumerator PlaySound(List<SoundQueue> soundQueye) {
			float timestart = Time.time;

			while (soundQueye.Count > 0) {
				SoundQueue sq = soundQueye.Find(x => x.time <= Time.time - timestart);
				if (sq != null) {
					sq.audioBlock.PlayRandom(this);
				}
				soundQueye.Remove(sq);
				yield return null;
			}

		}

		public void NextStep() {
			
			_step++;

			_activeBlock = _activeCatScene.catBlockList[_step];

			_catSceneUi.gameObject.SetActive(true);

			if (_soundCor != null) StopCoroutine(_soundCor);
			_soundCor = StartCoroutine(PlaySound(new List<SoundQueue>(_activeBlock.soundQueue)));
			_catSceneUi.SetData(_activeBlock);
		}

		private void EndCatScene() {
			if (_soundCor != null) StopCoroutine(_soundCor);

			if (_activeCatScene.isPause) Time.timeScale = 1;

			if (!_activeCatScene.noAutoSave)
				_activeCatScene.isShow = true;

			isActive = false;
			ExEvent.CatSceneEvent.EndCatScene.Call(_id);
			if (_catSceneUi != null) _catSceneUi.gameObject.SetActive(false);
			if (OnComplited != null) OnComplited();
		}

		private void Check(string num) {
			
		}

		public Dictionary<string, bool> Save() {

			Dictionary<string, bool> saveList = new Dictionary<string, bool>();

			foreach (var VARIABLE in library.catSceneList) {
				saveList.Add(VARIABLE.id, VARIABLE.isShow);
			}

			return saveList;

		}

		public void Load(Dictionary<string, bool> loadList) {

			//Dictionary<string, bool> saveList = new Dictionary<string, bool>();

			foreach (var VARIABLE in library.catSceneList) {
				if (loadList.ContainsKey(VARIABLE.id))
					VARIABLE.isShow = loadList[VARIABLE.id];
			}
		}

		public void Reset() {
			library.catSceneList.ForEach(x=>x.isShow = false);
		}

	}
	
}


