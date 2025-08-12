using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using it.Game.Managers;
using DG.Tweening;

namespace it.UI.Game
{
  public class HealthProgress : MonoBehaviourBase
  {
	 [SerializeField]
	 private float _maxWidth;
	 [SerializeField]
	 private float _minWidth;

	 [SerializeField]
	 private Image[] _items;

	 private float _speed = 1;
	 private int _targetValue;
	 private int _currentValue;

	 [SerializeField]
	 private RectTransform _line;

	 private Coroutine _update;

	 private float PectItem
	 {
		get
		{
		  return 1f / _items.Length;
		}
	 }

	 private void OnEnable()
	 {
		SetValue(true);
		it.Game.Events.EventDispatcher.AddListener(it.Game.Player.Stats.Health.EVT_HEALTH_CHANGE, HealthChecnge);
		StartCoroutine(CoroutineChange());
	 }

	 private void OnDisable()
	 {

		it.Game.Events.EventDispatcher.RemoveListener(it.Game.Player.Stats.Health.EVT_HEALTH_CHANGE, HealthChecnge);
		StopAllCoroutines();
	 }
	 private void HealthChecnge(com.ootii.Messages.IMessage rMessage)
	 {
		SetValue();
		//GameManager.Instance.UserManager.Health.Value;
	 }

	 private void SetValue(bool force = false)
	 {
		_targetValue = (int)Mathf.Ceil(GameManager.Instance.UserManager.Health.HealthPercent / PectItem);
		if (force)
		{
		  for(int i = 0; i < _items.Length; i++)
		  {
			 _items[i].color = (i <= _targetValue) ? Color.white : new Color(1, 1, 1, 0);
		  }

		  //_line.sizeDelta = new Vector2(_minWidth + ((_maxWidth - _maxWidth) * GameManager.Instance.UserManager.Health.HealthPercent), _line.sizeDelta.y);
		  //_currentValue = _targetValue;
		}

	 }

	 [ContextMenu("Set 1")]
	 private void Custon1()
	 {
		_targetValue = (int)Mathf.Ceil(1 / PectItem);
	 }

	 [ContextMenu("Set 0")]
	 private void Custon0()
	 {
		_targetValue = (int)Mathf.Ceil(0 / PectItem);
	 }

	 [ContextMenu("Set 0.5")]
	 private void Custon05()
	 {
		_targetValue = (int)Mathf.Ceil(0.5f / PectItem);
	 }

	 IEnumerator CoroutineChange()
	 {
		while (true)
		{
		  if (_currentValue != _targetValue)
		  {
			 int delta = _targetValue - _currentValue;
			 int inc = (int)Mathf.Sign(delta);
			 _currentValue += inc;

			 if (delta > 0)
			 {
				_items[_currentValue - 1].DOColor(Color.white, 0.05f);
			 }
			 else
			 {
				_items[_currentValue].DOColor(new Color(1, 1, 1, 0), 0.05f);
			 }
			 yield return new WaitForSeconds(0.05f);
		  }
		  yield return null;
		}
	 }

	 //private void Update()
	 //{
		//if (_currentValue != _targetValue)
		//{

		//  _currentValue += Mathf.Sign(delta) * (_speed * Time.deltaTime);

		//  if (delta > 0)
		//  {
		//	 _currentValue = Mathf.Clamp(_currentValue, 0, _targetValue);
		//  }
		//  else
		//  {
		//	 _currentValue = Mathf.Clamp(_currentValue, _targetValue, 1);
		//  }

		//  _line.sizeDelta = new Vector2(_minWidth + ((_maxWidth - _minWidth) * _currentValue), _line.sizeDelta.y);
		//}
	 //}

#if UNITY_EDITOR

	 [SerializeField]
	 private Transform _parentItems;
	 [SerializeField]
	 private RectTransform _oneItem;
	 [SerializeField]
	 private float _width = 8;
	 [SerializeField]
	 private float _distance = 200;

	 [ContextMenu("Spawn")]
	 private void Spawn()
	 {
		RectTransform[] rects = _parentItems.gameObject.GetComponentsInChildren<RectTransform>();

		for (int i = 0; i < rects.Length; i++)
		{
		  if (!rects[i].Equals(_oneItem) && !rects[i].Equals(_parentItems.GetComponent<RectTransform>()))
			 DestroyImmediate(rects[i].gameObject);
		}

		int index = 1;
		float distX = _width;
		while (index * distX < _distance)
		{
		  GameObject inst = Instantiate(_oneItem.gameObject, _parentItems);
		  inst.name += " " + index;
		  RectTransform rect = inst.GetComponent<RectTransform>();
		  rect.parent = _oneItem.parent;
		  rect.anchoredPosition = _oneItem.anchoredPosition;
		  rect.sizeDelta = _oneItem.sizeDelta;
		  Vector2 rectPos = rect.anchoredPosition;
		  rectPos.x += index * distX;
		  rect.anchoredPosition = rectPos;
		  index++;
		}
	 }

#endif
  }
}