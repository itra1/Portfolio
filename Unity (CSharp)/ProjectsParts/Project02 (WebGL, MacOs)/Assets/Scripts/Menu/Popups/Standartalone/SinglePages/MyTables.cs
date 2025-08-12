using UnityEngine;
using UnityEngine.UI;
using it.Network.Rest;

namespace it.Main.SinglePages
{
	public interface MyTableRecord
	{
		void SetData(Table table);
		public UnityEngine.Events.UnityAction<Table> OnOpenEvent { get; set; }
		public UnityEngine.Events.UnityAction OnSizeChangeEvent { get; set; }
	}

	public class MyTables : SinglePage
	{
		[SerializeField] private ScrollRect _scrollRect;

		private PoolList<MonoBehaviour> _pooler;
		private MonoBehaviour _recordPrefab;
		private ContentSizeFitter _csf;

		private void Awake()
		{
			_recordPrefab = _scrollRect.content.gameObject.GetComponentInChildren<MyTableRecord>(true) as MonoBehaviour;
			_csf = _scrollRect.content.GetComponent<ContentSizeFitter>();
		}

		protected override void EnableInit()
		{
			Garilla.Managers.ActiveTableManager.OnListChange += ListTablesChange;
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WindowsTableListChange, WindowsTableListChange);
			base.EnableInit();

			if (_recordPrefab == null)
				_recordPrefab = _scrollRect.content.gameObject.GetComponentInChildren<MyTableRecord>(true) as MonoBehaviour;

			(_recordPrefab as MonoBehaviour).gameObject.SetActive(false);

			SpawnItems();
		}

		private void OnDisable()
		{
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WindowsTableListChange, WindowsTableListChange);
			Garilla.Managers.ActiveTableManager.OnListChange -= ListTablesChange;
		}

		private void ListTablesChange()
		{
			SpawnItems();
		}

		//private void WindowsTableListChange(com.ootii.Messages.IMessage handle){
		//	SpawnItems();
		//}

		private void SpawnItems()
		{

			if (UserController.Instance == null) return;

			if (_pooler == null)
				_pooler = new PoolList<MonoBehaviour>(_recordPrefab.gameObject, _scrollRect.content);
			_pooler.HideAll();

			//var tables = StandaloneController.Instance.TableWindows;
			var sessions = UserController.Instance.ActiveTableManager.ActiveSessions;

			for (int i = 0; i < sessions.Count; i++)
			{
				var table = TableManager.Instance.TableList.Find(x => x.id == (ulong)sessions[i].table_id);
				if (table == null) continue;

				var itm = _pooler.GetItem().GetComponent<MyTableRecord>();
				itm.SetData(table);
				itm.OnOpenEvent = (tab) =>
				{
#if !UNITY_STANDALONE
					OpenAll();
					return;
#endif
					TableManager.Instance.OpenTable(tab.id);
					Hide();
					//#if UNITY_STANDALONE
					//					var windowData = StandaloneController.Instance.TableWindows.Find(x => x.Id == tab.Id);
					//					if (windowData != null)
					//					{
					//						StandaloneController.Instance.FocusWindow(tab);
					//					}
					//					else
					//					{
					//						TableManager.Instance.OpenTable(tab.Id);
					//					}
					//#else

					//					OpenTableManager.Instance.OpenGame(tab);
					//#endif
				};

#if !UNITY_STANDALONE
				itm.OnSizeChangeEvent = () =>
				{
					_csf.enabled = false;
					_csf.enabled = true;
				};

#endif

			}

#if !UNITY_STANDALONE
			var _recRt = _recordPrefab.GetComponent<RectTransform>();
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, _recRt.rect.height * sessions.Count);

#endif
		}

		private void OpenAll()
		{
			var sessions = UserController.Instance.ActiveTableManager.ActiveSessions;
			for (int i = 0; i < sessions.Count; i++)
			{
				TableManager.Instance.OpenTable(sessions[i].table_id);
			}
			Hide();
		}

	}
}