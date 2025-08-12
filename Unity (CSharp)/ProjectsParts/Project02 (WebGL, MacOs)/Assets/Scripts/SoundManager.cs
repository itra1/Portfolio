using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource SourceBackground;
    [SerializeField] private AudioSource SourceDealer;
    [SerializeField] private AudioSource SourceEffect;

    [Space]
    [SerializeField] private AudioMixer MixerBackground;
    [SerializeField] private AudioMixer MixerDealer;
    [SerializeField] private AudioMixer MixerEffect;
    [SerializeField] private Vector2 minMaxVolume = new Vector2(-40f, 40f);

    [Space, Header("Clips")]
    [SerializeField] AudioClip IntroClip;

    public static SoundManager instance;
    public static SoundGet sound;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        sound = GameHelper.UserProfile.sound;
        SetVolumeBackground(sound.background / 100f);
        SetVolumeDealer(sound.dealer_voice / 100f);
        SetVolumeEffect(sound.sound_effect / 100f);
        SoundOnOff(sound.on);
        MuteCriticalOn(sound.mute_critical_alert);
        PlaySoundIntro();
    }

    public void SetVolumeBackground(float vl)
    {
        sound.background = (int)(vl * 100);
        MixerBackground.SetFloat("Volume", (sound.on == false || vl <= 0) ? -80f : Mathf.Lerp(minMaxVolume.x, minMaxVolume.y, vl));
    }

    public void SetVolumeDealer(float vl)
    {
        sound.dealer_voice = (int)(vl * 100);
        MixerDealer.SetFloat("Volume", (sound.on == false || vl <= 0) ? -80f : Mathf.Lerp(minMaxVolume.x, minMaxVolume.y, vl));
    }

    public void SetVolumeEffect(float vl)
    {
        sound.sound_effect = (int)(vl * 100);
        MixerEffect.SetFloat("Volume", (sound.on == false || vl <= 0) ? -80f : Mathf.Lerp(minMaxVolume.x, minMaxVolume.y, vl));
    }

    public void SoundOnOff(bool on)
    {
        sound.on = on;
        SetVolumeBackground(sound.background / 100f);
        SetVolumeDealer(sound.dealer_voice / 100f);
        SetVolumeEffect(sound.sound_effect / 100f);
    }

    public void MuteCriticalOn(bool on)
    {
        sound.mute_critical_alert = on;
    }


    private void Play(AudioSource source, AudioClip clip, bool ones = false, bool loop = false)
    {
        source.loop = loop;
        if (ones)
        {
            source.PlayOneShot(clip);
        }
        else
        {
            source.clip = clip;
            source.Play();
        }
    }

    public void PlayBackground(AudioClip clip, bool ones = false, bool loop = false)
    {
        Play(SourceBackground, clip, ones, loop);
    }

    public void PlayDealer(AudioClip clip, bool ones = false, bool loop = false)
    {
        Play(SourceDealer, clip, ones, loop);
    }

    public void PlayEffect(AudioClip clip, bool ones = false, bool loop = false)
    {
        Play(SourceEffect, clip, ones, loop);
    }


    public void PlaySoundIntro ()
    {
        if (IntroClip == null) return;
        PlayBackground(IntroClip, true);
    }
}
