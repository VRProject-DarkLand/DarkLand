using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour, IGameManager {
    [SerializeField] private AudioSource uiSoundSource;
    [SerializeField] private AudioSource musicSource;
[SerializeField] private string introBGMusic;
[SerializeField] private string levelBGMusic;


    public ManagerStatus status {get; private set;}
    public void Startup() {
        Debug.Log("Audio manager starting...");
        musicSource.ignoreListenerVolume = true;
        musicSource.ignoreListenerPause = true;
        uiSoundSource.ignoreListenerPause = true;
        soundVolume = 1f;
        musicVolume = 1f;
        uiSoundSource.mute = AudioListener.pause;
        PlayIntroMusic();
        status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip clip) {
        uiSoundSource.PlayOneShot(clip);
    }

    public float musicVolume {
        get {
            return musicSource.volume;
        }
        set {
            if (musicSource != null) {
                musicSource.volume = value;
            }
        }
    }

    public bool musicOn {
            get {
                if (musicSource != null) {
                    return !musicSource.mute;
                }
                return false;
            }
        set {
            if (musicSource != null) {
                musicSource.mute = !value;
            }
        }
    }


    public bool AllSoundOn{
        get{return uiSoundOn && soundOn;}
        set{uiSoundOn = value; soundOn = value;}
    }

    public bool uiSoundOn {
        get {return !uiSoundSource.mute;}
        set {uiSoundSource.mute = !value;}
    }

    public float soundVolume {
        get {return AudioListener.volume;}
        set {AudioListener.volume = value;}
    }
    public bool soundOn {
        get {return !AudioListener.pause;}
        set {AudioListener.pause = !value;}
    }

    public void PlayIntroMusic() {
        PlayMusic((AudioClip)Resources.Load("Music/"+introBGMusic));
    }
    public void PlayLevelMusic() {
        PlayMusic((AudioClip)Resources.Load("Music/"+levelBGMusic));
    }
    private void PlayMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void StopMusic() {
        musicSource.Stop();
    }   

}
