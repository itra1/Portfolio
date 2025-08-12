using Engine.Scripts.Base;
using Game.Scripts.Providers.Songs.Helpers;
using Game.Scripts.UI.Presenters;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class SongChooserPanelHandler : MonoBehaviour, IInjection
	{
		[SerializeField] protected CollectionTracksPresenter m_SongChooserPanel;
		[SerializeField] protected string m_PlayButtonName = "Submit";
		[SerializeField] protected string m_NavigationAxisName = "Vertical";
		[SerializeField] protected float m_RepeatNavigationInput = 0.5f;

		[SerializeField] private Button _buttonPlay;

		private ISongsHelper _songHelper;
		protected float m_RepeatInputNavigationTimer = 0;

		[Inject]
		private void Construuctor(ISongsHelper songHelper)
		{
			_songHelper = songHelper;

			if (m_SongChooserPanel == null)
				m_SongChooserPanel = GetComponent<CollectionTracksPresenter>();
		}

		public void Update()
		{
			if (m_SongChooserPanel.IsOpen == false)
				return;

			NavigationInputs();

			if (Input.GetButtonDown(m_PlayButtonName) && _buttonPlay.interactable)
			{
				//m_SongChooserPanel.PlaySelectedSong();
			}
		}

		protected virtual void NavigationInputs()
		{
			float axisValue = Input.GetAxisRaw(m_NavigationAxisName);

			var next = axisValue < -0.1f;
			var previous = axisValue > 0.1f;

			if (!next && !previous)
			{
				m_RepeatInputNavigationTimer = -1;
				return;
			}

			if (m_RepeatInputNavigationTimer > 0)
			{
				m_RepeatInputNavigationTimer -= Time.deltaTime;
				return;
			}

			m_RepeatInputNavigationTimer = m_RepeatNavigationInput;
			SelectNextPrevious(next);
		}

		protected virtual void SelectNextPrevious(bool isNext)
		{
			//var currentIndex = m_SongChooserPanel.SelectedIndex;
			//var newIndex = isNext ? currentIndex + 1 : currentIndex - 1;

			//var songCount = _songHelper.Songs.Count;
			//if (newIndex >= songCount)
			//{
			//	newIndex = 0;
			//}
			//else if (newIndex <= 0)
			//{
			//	newIndex = songCount - 1;
			//}

			//m_SongChooserPanel.SetSelectedSong(newIndex);
		}
	}
}