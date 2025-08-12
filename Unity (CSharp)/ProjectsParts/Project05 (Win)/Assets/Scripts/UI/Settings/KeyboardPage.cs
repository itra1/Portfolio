using System.Collections;
using it.Game.Managers;
using UnityEngine;

namespace it.UI.Settings
{
  public class KeyboardPage : PageBase
  {
	 [SerializeField] private InputKey _inputItem;
	 [SerializeField] private RectTransform _content;

	 private InputKey _selectedKey;
	 private bool _existsChange;

	 private void Start()
	 {
		SpawnKeys();
	 }

	 protected override void OnEnable()
	 {
		base.OnEnable();
		_inputItem.gameObject.SetActive(false);
	 }

	 protected override void OnDisable()
	 {
		base.OnDisable();
		if (_existsChange)
		{
		  _existsChange = false;
		  it.Game.Managers.GameManager.Instance.GameKeys.Save();
		}
	 }

	 private void SpawnKeys()
	 {
		var keys = GameManager.Instance.GameKeys.Keys;

		_content.sizeDelta = new Vector2(_content.sizeDelta.x, 50 * keys.Count);

		RectTransform trPrefab = _inputItem.GetComponent<RectTransform>();

		for (int i = 0; i < keys.Count; i++)
		{
		  GameObject item = Instantiate(_inputItem.gameObject, _inputItem.transform.parent);
		  var key = item.GetComponent<InputKey>();
		  key.SetKey(keys[i]);
		  item.gameObject.SetActive(true);
		  item.GetComponent<RectTransform>().anchoredPosition = new Vector3(trPrefab.anchoredPosition.x, (_content.sizeDelta.y / 2 - 25) - 50 * i);
		  key.OnClick = ItemClick;
		}

	 }

	 public void ItemClick(InputKey inputKey)
	 {
		_selectedKey = inputKey;
		_selectedKey.SetWaitKey();

	 }

	 private void Update()
	 {
		if (_selectedKey != null)
		{
		  if (Input.anyKey)
		  {
			 foreach (var key in com.ootii.Input.EnumInput.EnumNames.Keys)
			 {
				if (GameManager.Instance.EnvironmentInputSource.IsJustPressed(key))
				{
				  if (_selectedKey.KeyData.Value != key)
					 _existsChange = true;

				  _selectedKey.KeyData.Value = key;
				  _selectedKey.SetKey();
				  _selectedKey = null;
				  return;
				}
			 }
		  }
		}
	 }
  }
}