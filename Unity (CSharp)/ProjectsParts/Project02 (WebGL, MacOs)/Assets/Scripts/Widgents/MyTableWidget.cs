using UnityEngine;
using TMPro;

namespace it.Widgets
{
	public class MyTableWidget : MonoBehaviour
	{
		[SerializeField] private GameObject _indgicatorIcone;
		[SerializeField] private TextMeshProUGUI _indicatorValue;

		private void OnEnable()
		{
			//com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WindowsTableListChange, WindowsTableListChange);
			Garilla.Managers.ActiveTableManager.OnListChange -= ListTablesChange;
			Garilla.Managers.ActiveTableManager.OnListChange += ListTablesChange;
			ListTablesChange();
		}

		private void OnDisable()
		{
			//com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WindowsTableListChange, WindowsTableListChange);
			Garilla.Managers.ActiveTableManager.OnListChange -= ListTablesChange;
		}

		private void ListTablesChange()
		{
			FillIndicator();
		}

		//private void WindowsTableListChange(com.ootii.Messages.IMessage handler)
		//{
		//	FillIndicator();
		//}

		private void FillIndicator()
		{
			if (UserController.Instance == null) return;
			if (UserController.Instance?.ActiveTableManager.ActiveSessions.Count <= 0)
			{
				_indgicatorIcone.SetActive(false);
			}
			else
			{
				_indgicatorIcone.SetActive(true);
				_indicatorValue.text = UserController.Instance.ActiveTableManager.ActiveSessions.Count.ToString();
			}
		}

		public void ClickTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.MyTables);
		}

	}
}