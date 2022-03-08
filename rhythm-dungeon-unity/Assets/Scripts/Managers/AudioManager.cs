using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; //Singleton

    public AudioSource background, soundEffect, drum, itemSoundEffect;

    [HideInInspector]
    public bool isMusicPlay; //for checking music status

    [Header("AudioClips")] //List of sounds
    public AudioClip aiDamage;
    public AudioClip playerDamage;

    public AudioClip pickUpKey;
    public AudioClip pickUpItems;
    public AudioClip pickUpCoin;
    public AudioClip openDoor;
    public AudioClip drinkBottle;

    [Header("Musics")]
    public AudioClip[] music;

    [Header("Drums")]
    public AudioClip[] drums;

    [Header("Item")]
    public AudioClip lightning;

    [HideInInspector]
    public readonly Dictionary<PlayerAction, int> actionDumpIndex = new Dictionary<PlayerAction, int>() {
        { PlayerAction.MoveUp, 0 },
        { PlayerAction.MoveDown, 2 },
        { PlayerAction.Charge, 1 },
        { PlayerAction.Attack, 3 },
        { PlayerAction.UseItem_0, 4},
        { PlayerAction.UseItem_1, 5}
    };

    float drumOriginalVolume;

    //Singleton
    private void Awake()
    {
        if (AudioManager.Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        drumOriginalVolume = drum.volume;
    }

    public void Pause()
    {
        background.Pause();
        soundEffect.Pause();
    }

    public void Continue()
    {
        background.UnPause();
        soundEffect.UnPause();
    }

    public void PlayMusic(int musicIndex)
    {
        if (!isMusicPlay) //if the music is not playing
        {
            isMusicPlay = true;
            Play(background, music[musicIndex], true); //Play music
        }
        else
        {
            background.Stop();
            background.clip = music[musicIndex];
            background.Play();
        }
    }

    public void PlayMusic(AudioClip music)
    {
        if (!isMusicPlay) //if the music is not playing
        {
            isMusicPlay = true;
            Play(background, music, true); //Play music
        }
        else
        {
            background.Stop();
            background.clip = music;
            background.Play();
        }
    }

    public void StopMusic()
    {
        isMusicPlay = false;
        background.Stop();
    }

    public void Play(AudioSource audioSource, AudioClip audioClip, bool loop)
    {
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void MusicFadeOut()
    {
        StartCoroutine(MusicFadeOutCoroutine());
    }

    IEnumerator MusicFadeOutCoroutine()
    {
        var originalVolume = background.volume;
        var time = GameManager.Instance.secondPerHalfBeat * 8f;
        var velocity = originalVolume / time;
        var deltaTime = 0f;
        while (deltaTime < time)
        {
            deltaTime += Time.deltaTime;
            background.volume = Mathf.Max(background.volume - velocity * Time.fixedDeltaTime, 0f);
            yield return new WaitForFixedUpdate();
        }
        background.Stop();
        background.volume = originalVolume;
        isMusicPlay = false;
    }

    public void PlayDrum(int index, float volume_factor = 1f)
    {
        if (index >= drums.Length) return;

        drum.volume = drumOriginalVolume * volume_factor;
        drum.clip = drums[index];
        drum.Play();
    }

    public void PlayLightning()
    {
        Play(itemSoundEffect, lightning, false);
    }
}
