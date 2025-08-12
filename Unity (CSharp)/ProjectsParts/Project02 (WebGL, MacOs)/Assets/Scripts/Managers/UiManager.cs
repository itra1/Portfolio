using it.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Settings;
using UnityEngine.UI;
using TMPro;

public class UiManager : Singleton<UiManager>
{
	[SerializeField] private RectTransform _uibase;
	[SerializeField] private RectTransform _headerRect;

	public AspectRatioFitter AspectRationFilder;

	public TextMeshProUGUI VersionLabel { get => _versionLabel; set => _versionLabel = value; }

	private List<UIPanel> _guiList = new List<UIPanel>();
	private UILibrary _library;
	[SerializeField] private TMPro.TextMeshProUGUI _versionLabel;

	public UILibrary Library
	{
		get
		{
			if (_library == null)
				_library = (UILibrary)Garilla.ResourceManager.GetResource<UILibrary>(AppSettings.Folders.UILibrary);
			//_library = Resources.Load<UILibrary>(Settings.Folders.UILibrary);
			return _library;
		}
	}

	public RectTransform HeaderRect { get => _headerRect; set => _headerRect = value; }
	public RectTransform Uibase { get => _uibase; set => _uibase = value; }

	private void Awake()
	{
		if (_versionLabel != null)
			_versionLabel.text = Application.version;
	}
	public void OpenLobby(){
		var panel = GetPanel<it.UI.MainPanel>(_uibase);
		panel.gameObject.SetActive(true);
	}

	public GamePanel OpenGame()
	{
		var panel = GetPanel<it.UI.GamePanel>(_uibase);
		panel.gameObject.SetActive(true);
		return panel;
	}

	public static T GetPanel<T>() where T : UIPanel
	{
		return Instance.GetPanel_<T>(null);
	}
	public static T GetPanel<T>(Transform parent) where T : UIPanel
	{
		return Instance.GetPanel_<T>(parent);
	}

	private T GetPanel_<T>(Transform parent) where T : UIPanel
	{

		T inst = (T)_guiList.Find(x => x.GetType() == typeof(T));

		if (inst == null)
		{

			T lib = (T)Library.DialogLibrary.Find(x => x.GetType() == typeof(T));

			if (lib == null)
			{
				it.Logger.LogError("Не найдена панель типа " + typeof(T));
				return null;
			}
			lib.gameObject.SetActive(false);

			GameObject panel = InstantiatePrefab(lib.gameObject, parent);

			if (panel == null)
			{
				it.Logger.LogError("Не удалось сгенерировать панель с типом " + typeof(T));
				return null;
			}

			inst = panel.GetComponent<T>();

			_guiList.Add(inst);

		}

		inst.transform.SetAsLastSibling();
		return inst;
	}
	GameObject InstantiatePrefab(GameObject prefabUi, Transform parent)
	{
		prefabUi.gameObject.SetActive(false);
		GameObject inst = Instantiate(prefabUi, parent);
		return inst;
	}

}
