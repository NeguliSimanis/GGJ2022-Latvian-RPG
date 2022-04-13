using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicTrack
{
    Ogotu1,
    Medniex1,
    Medniex2,
    undefined
}

public enum AudioType
{
    Music,
    SFX
}

public class AudioManager : MonoBehaviour
{
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float lastMusicLength;
    private float nextMusicStartTime;
    private AudioClip nextBackgroundMusic;

    #region MUSIC
    [Header("Music")]
    [SerializeField]
    AudioSource musicAudioSource;
    [SerializeField]
    AudioClip ogotu1;
    [SerializeField]
    AudioClip medniex1;
    [SerializeField]
    AudioClip medniex2;
    MusicTrack currMusicTrack = MusicTrack.Ogotu1;
    #endregion

    #region SFX
    [Header("sfx's")]
    [SerializeField]
    AudioSource audioSource;


    [SerializeField]
    AudioClip buttonSFX;

    [SerializeField]
    AudioClip levelUpSFX;
    [SerializeField]
    AudioClip hurtSFX;

    [SerializeField]
    AudioClip lightVictorySFX;
    [SerializeField]
    AudioClip darkVictorySFX;
    [SerializeField]
    AudioClip defeatSFX;

    [SerializeField]
    AudioClip playerTurnSFX;
    [SerializeField]
    AudioClip enemyTurnSFX;
    [SerializeField]
    AudioClip neutralTurnSFX;

    [SerializeField]
    AudioClip utilityAbilitySFX;
    [SerializeField]
    AudioClip attackSound;

    [SerializeField]
    AudioClip[] stepSounds;
    int stepSoundCount;
    #endregion

    private void Start()
    {
        stepSoundCount = stepSounds.Length;
    }

    public void PlayStepSound()
    {
        int soundID = Random.Range(0, stepSoundCount);
        audioSource.PlayOneShot(stepSounds[soundID], sfxVolume);
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound, sfxVolume);
    }

    public void PlayButtonSFX()
    {
        audioSource.PlayOneShot(buttonSFX, sfxVolume);
    }

    public void PlayDefeatSFX()
    {
        audioSource.PlayOneShot(defeatSFX, sfxVolume);
    }

    public void PlayHurtSFX()
    {
        audioSource.PlayOneShot(hurtSFX, sfxVolume);
    }

    public void PlayLevelupSFX()
    {
        audioSource.PlayOneShot(levelUpSFX, sfxVolume);
    }

    public void PlayUtilitySFX()
    {
        audioSource.PlayOneShot(utilityAbilitySFX, sfxVolume);
    }

    public void PlayPlayerTurnSFX()
    {
        audioSource.PlayOneShot(playerTurnSFX, sfxVolume);
    }

    public void PlayNeutralTurnSFX()
    {
        audioSource.PlayOneShot(neutralTurnSFX, sfxVolume);
    }

    public void PlayEnemyTurnSFX()
    {
        audioSource.PlayOneShot(enemyTurnSFX, sfxVolume);
    }

    public void PlayLightVictorySFX()
    {
        audioSource.PlayOneShot(lightVictorySFX, sfxVolume);
    }

    public void PlayDarkVictorySFX()
    {
        Debug.Log("DARK FS");
        audioSource.PlayOneShot(darkVictorySFX, sfxVolume * 1.5f);
    }


    public void ManageMusicSwitch()
    {
        if (GameData.current.dungeonFloor == 2)
            StartCoroutine(FadeToDifferentMusic(0.4f, silenceDuration: 0.5f,
                nextMusic: medniex1));
        if (GameData.current.dungeonFloor == 5)
        {
            StartCoroutine(FadeToDifferentMusic(0.4f, silenceDuration: 0.5f,
                nextMusic: ogotu1, targetTrack: MusicTrack.Ogotu1));
            nextMusicStartTime = Time.time + ogotu1.length;
            nextBackgroundMusic = medniex1;
        }
        else if (Time.time > nextMusicStartTime && GameData.current.dungeonFloor > 5)
        {
            if (nextBackgroundMusic == medniex1)
            {
                nextBackgroundMusic = medniex2;
                nextMusicStartTime = Time.time + medniex1.length;
                StartCoroutine(FadeToDifferentMusic(0.4f, silenceDuration: 0.5f,
                    nextMusic: medniex1, targetTrack: MusicTrack.Medniex1));
            }
            else if (nextBackgroundMusic == medniex2)
            {
                nextBackgroundMusic = ogotu1;
                nextMusicStartTime = Time.time + medniex2.length;
                StartCoroutine(FadeToDifferentMusic(0.4f, silenceDuration: 0.5f,
                    nextMusic: medniex2, targetTrack: MusicTrack.Medniex2));
            }
            else // ogotu1
            {
                nextBackgroundMusic = medniex1;
                nextMusicStartTime = Time.time + ogotu1.length;
                StartCoroutine(FadeToDifferentMusic(0.4f, silenceDuration: 0.5f,
                    nextMusic: ogotu1, targetTrack: MusicTrack.Ogotu1));
            }
        }
    }

    public IEnumerator FadeToDifferentMusic(float duration, float silenceDuration, AudioClip nextMusic, 
        MusicTrack targetTrack = MusicTrack.undefined)
    {
        float currentTime = 0;
        float start = musicAudioSource.volume;

        AudioClip targetAudioClip = medniex1;
        if (targetTrack == MusicTrack.Ogotu1)
        {
            targetAudioClip = ogotu1;
        }
        if (targetTrack == MusicTrack.Medniex2)
        {
            targetAudioClip = medniex2;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }

        currentTime = 0;
        while (currentTime < silenceDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        musicAudioSource.clip = targetAudioClip;
        musicAudioSource.Play();
        currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(start, musicVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void SetVolume(float volume, AudioType audioType)
    {
        if (audioType == AudioType.Music)
        {
            Debug.LogError("music baby");
            musicAudioSource.volume = volume * 0.01f;
            musicVolume = volume * 0.01f;
        }
        else if (audioType == AudioType.SFX)
        {
            Debug.LogError("setting sfx babvy");
            sfxVolume = volume * 0.01f;
        }
    }
}
