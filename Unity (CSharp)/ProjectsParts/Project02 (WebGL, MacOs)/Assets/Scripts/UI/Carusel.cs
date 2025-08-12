using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Garilla.Main
{
	public class Carusel : MonoBehaviour
	{
		[SerializeField] protected List<ImageStruct> _banners;
		[SerializeField] protected RectTransform _leftRect;
		[SerializeField] protected RectTransform _rightRect;
		[SerializeField] protected RectTransform _centerRect;
		[SerializeField] protected RectTransform _bubushkaParent;
		[SerializeField] protected Bubushka _bubushkaPrefab;
		[SerializeField] protected Color _colorBack;
		[SerializeField] protected RectTransform _itemPrefab;
		[SerializeField] protected RectTransform _parentImages;

		private string _currentLoc;
		protected List<Bubushka> _bubushkaList = new List<Bubushka>();
		protected int _index = 0;

		protected float _timeChnage = 5;
		protected Coroutine _timerCoroutine;

		private void LoadBannersToCurrentLoc()
		{
			if (I2.Loc.LocalizationManager.CurrentLanguageCode == _currentLoc) return;

			_currentLoc = I2.Loc.LocalizationManager.CurrentLanguageCode;

			_banners.ForEach(x => Destroy(x.Banner.gameObject));
			_banners.Clear();

			Texture2D[] textures = Garilla.ResourceManager.GetResourceAll<Texture2D>($"Textures/Banners/Carusel/{_currentLoc}");

			for(int i = 0; i < textures.Length;i++){

#if UNITY_IOS

				List<string> nameList = new List<string>() { "01", "03", "05" };

				if (nameList.Contains(textures[i].name)) continue;


#endif


				GameObject inst = Instantiate(_itemPrefab.gameObject, _parentImages);
				inst.gameObject.SetActive(true);
				RectTransform iRect = inst.GetComponent<RectTransform>();
				RawImage rawI = inst.GetComponentInChildren<RawImage>();
				rawI.texture = textures[i];
				_banners.Add(new ImageStruct() { Banner = iRect, Image = rawI });

			}

		}

		private void OnEnable()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);
			OnLocalizeChange();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
		}

		protected virtual void LocalizeChangeHadler(com.ootii.Messages.IMessage handle)
		{
			OnLocalizeChange();
		}

		protected virtual void OnLocalizeChange()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

			LoadBannersToCurrentLoc();

			//BubushkasSpawn();
			FirstInit();

			PlayWaitChange();
		}

		private void BubushkasSpawn()
		{
			_bubushkaPrefab.gameObject.SetActive(false);
			_bubushkaList.ForEach(x => x.gameObject.SetActive(false));

			for (int i = 0; i < _banners.Count; i++)
			{

				var cm = _bubushkaList.Find(x => !x.gameObject.activeSelf);

				if (cm == null)
				{
					GameObject go = Instantiate(_bubushkaPrefab.gameObject, _bubushkaParent);
					cm = go.GetComponent<Bubushka>();
					_bubushkaList.Add(cm);
				}

				cm.Fill(false);
				cm.gameObject.SetActive(true);

			}
			_bubushkaParent.sizeDelta = new Vector2(15f * _banners.Count, _bubushkaParent.sizeDelta.y);
		}

		private IEnumerator TimerChnage()
		{
			yield return new WaitForSeconds(_timeChnage);
			RightButton();
		}

		private void PlayWaitChange()
		{
			_timerCoroutine = StartCoroutine(TimerChnage());
		}

		private void UnfillBubushka()
		{
			//_bubushkaList.ForEach(x => x.Fill(false));
		}

		public void LeftButton()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

			_index = DecrementValue();
			ConfirmPosition();

			PlayWaitChange();
		}
		public void RightButton()
		{
			if (_timerCoroutine != null)
				StopCoroutine(_timerCoroutine);

			_index = IncrementValue();
			ConfirmPosition();

			PlayWaitChange();
		}

		protected virtual void FirstInit() { }


		protected virtual void ConfirmPosition()
		{
			UnfillBubushka();
			//_bubushkaList[_index].Fill(true);

		}

		protected virtual int IncrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val++;
				if (val >= _banners.Count) val = 0;
			}
			return val;
		}

		protected virtual int DecrementValue(int inc = 1)
		{
			int val = _index;
			for (int i = 0; i < inc; i++)
			{
				val--;
				if (val < 0) val = _banners.Count - 1;
			}
			return val;
		}

		[System.Serializable]
		public struct ImageStruct
		{
			public RectTransform Banner;
			public RawImage Image;
		}

	}

}