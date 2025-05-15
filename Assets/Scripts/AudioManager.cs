using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceSFX;
    [SerializeField] private AudioSource audioSourceMusic;

    [Header("Audio SFX")]
    [SerializeField] public AudioClip sound_Campana;
    [SerializeField] public AudioClip sound_Cavar;
    [SerializeField] public AudioClip sound_Character_Death;
    [SerializeField] public AudioClip sound_Character_Electrified;
    [SerializeField] public AudioClip sound_Character_Hit;
    [SerializeField] public AudioClip sound_Menu_Play;
    [SerializeField] public AudioClip sound_Put_Card;
    [SerializeField] public AudioClip sound_Recover_Life;
    [SerializeField] public AudioClip sound_Time_Bar;
    [SerializeField] public AudioClip sound_Thunder;

    [Header("Audio Music")]
    [SerializeField] public AudioClip music_MainTheme;
    [SerializeField] public AudioClip music_MenuTheme;
    [SerializeField] public AudioClip music_BossTheme;

    private void Awake()
    {
        instance = this;


    }

    public void PlayOneShot(AudioClip audio, float volume = 0.75f)
    {
        audioSourceSFX.pitch = Random.Range(0.9f, 1.1f);

        audioSourceSFX.PlayOneShot(audio, volume);
    }

    public void PlayWithPitch(AudioClip audio, float pitch, float volume = 0.75f)
    {
        audioSourceSFX.pitch = pitch;

        audioSourceSFX.PlayOneShot(audio, volume);
    }

    public void ChangeMusicPitch(float pitch)
    {
        audioSourceMusic.pitch = pitch;
    }

    public void PlayMusic(AudioClip music)
    {
        audioSourceMusic.clip = music;
        audioSourceMusic.Play();
    }
    public void StopMusic()
    {
        audioSourceMusic.Stop();
    }

    public void IntenseMusicOnRound(int roundTime)
    {
        if (roundTime == 17)
            audioSourceMusic.pitch += 0.035f;

        else if (roundTime == 31)
            audioSourceMusic.pitch += 0.035f;
    }
    public void ResetPitch()
    {
        audioSourceMusic.pitch = 1;
    }

    private float[] volumeLevels = { 0.0001f, 0.33f, 0.66f, 1.0f };

    public void SetMusicVolume(int level)
    {
        float linearVolume = volumeLevels[level];
        float dB = Mathf.Log10(linearVolume) * 20f;
        mixer.SetFloat("Music_Volume", dB);
    }

    public void SetSFXVolume(int level)
    {
        float linearVolume = volumeLevels[level];
        float dB = Mathf.Log10(linearVolume) * 20f;
        mixer.SetFloat("SFX_Volume", dB);
    }
}
