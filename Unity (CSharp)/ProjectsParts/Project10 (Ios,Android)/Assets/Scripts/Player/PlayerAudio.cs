using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Jack {

  public class PlayerAudio: PlayerComponent {

    private AudioSource _audioComp;
    private AudioSource audioComp { get { return (_audioComp == null ? _audioComp = GetComponent<AudioSource>() : _audioComp); } }

    public AudioClip swordDefendClip;
    public AudioClip jumpOutClip;
    public AudioClip fallClip;
    public AudioClip[] bravoClip;
    public AudioClip damageEnemy;
    public AudioClip damageStone;
    public AudioClip magnetAuraClip;
    public AudioClip jumpAudio;
    public AudioClip jumpAudioAir;
    public AudioClip trapClip;
    public AudioClip swordClip;
    public AudioClip bombClip;
    public AudioClip molotovClip;
    public AudioClip gunClip;
    public AudioClip shipClip;

    private void Start() { }

    public void Play(AudioClip clip, AudioMixerTypes mixer) {
      AudioManager.PlayEffect(clip, mixer);
    }

    public void PlayOneShot(AudioClip clip, float value) {
      audioComp.PlayOneShot(clip, value);
    }

    public void PlayEffect(AudioClip clip, AudioMixerTypes mixer) {
      AudioManager.PlayEffect(clip, mixer);
    }

  }
}