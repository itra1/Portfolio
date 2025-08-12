using Game.Base;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using StringDrop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Game.Common
{
	[RequireComponent(typeof(Button))]
	public class ButtonAudio : MonoBehaviour, IInjection
	{

		[SerializeField]
		[StringDropList(typeof(SoundNames))]
		protected string _openAudioClip = SoundNames.UiTap;
		private AudioHandler _audioHandler;

		[Inject]
		public void Constructor(AudioHandler audioHandler)
		{
			_audioHandler = audioHandler;
		}

		public void Awake()
		{
			var btn = GetComponent<Button>();
			btn.onClick.AddListener(() =>
			{
				if (btn.interactable)
					_ = _audioHandler.PlayRandomClip(_openAudioClip);
			});
		}
	}
}
