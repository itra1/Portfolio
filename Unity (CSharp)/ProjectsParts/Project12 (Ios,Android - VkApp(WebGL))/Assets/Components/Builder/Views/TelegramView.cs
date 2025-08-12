using System.Collections.Generic;
using Builder.Common;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Builder.Views
{
	public class TelegramView : ViewBase
	{
		public UnityEvent<string> OnSendMessage = new();

		private VisualElement _root;
		private List<Record> _records = new();
		private ScrollView _scrollView;
		public override string Type => ViewsType.Telegram;

		public TelegramView(BuildSession buildData) : base(buildData)
		{
		}

		protected override void LoadPrefab()
		{
			_viewPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuilderWindow.TelegramViewTemplate);
		}

		protected override void CreateUi()
		{
			base.CreateUi();

			var messageField = _view.Q<TextField>("message");
			var button = _view.Q<Button>("sendbutton");
			button.clicked += () =>
			{
				OnSendMessage?.Invoke(messageField.text);
			};

			_buildData.BodyElement.Add(_view);
		}

		public override void Show()
		{
			base.Show();

			_records.Clear();

			foreach (var chat in _buildData.Settings.Telegrams)
			{
				_records.Add(new Record(chat, () =>
				{
					_buildData.Settings.Save();
				}));
			}

			VisibleRecords();
		}

		public void VisibleRecords()
		{
			_root ??= _buildData.Window.Root;
			if (_root == null)
				return;

			//_scrollView ??= _root.Q<ScrollView>("chatList");
			//if (_scrollView == null)
			//	return;

			//_scrollView.Clear();
			//foreach (Record record in _records)
			//{
			//	_scrollView.Add(record.GetRecord(_viewPrefab));
			//}
			//_scrollView.style.height = 0;
		}

		public void Message(string message)
		{
			foreach (Record record in _records)
			{
				//Debug.Log(record.IsSend);
				//if (record.IsSend)
				//	Request(record.Id, message);
			}
		}

		class Record
		{
			public string Title => _chat.Title;
			public bool IsSend => _chat.IsSend;
			public string Id => _chat.Id;
			public VisualElement UIElement;

			private Settings.TelegramChat _chat;
			private UnityAction OnChnage;

			public Record(Settings.TelegramChat chat, UnityAction onChange)
			{
				_chat = chat;
				OnChnage = onChange;
			}

			public VisualElement GetRecord(VisualTreeAsset prefab)
			{
				UIElement ??= prefab.Instantiate();
				var recordLabel = UIElement.Q<Label>("label");
				var selectCheckbos = UIElement.Q<Toggle>("isSelect");
				//recordLabel.text = Title;
				//selectCheckbos.value = IsSend;

				_ = selectCheckbos.RegisterValueChangedCallback<bool>((res) =>
				{
					selectCheckbos.value = res.newValue;
					_chat.IsSend = res.newValue;
					OnChnage?.Invoke();
				});

				return UIElement;

			}
		}
	}
}