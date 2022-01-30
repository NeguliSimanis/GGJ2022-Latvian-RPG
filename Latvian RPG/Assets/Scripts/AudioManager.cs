using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private float sfxVolume = 1f;
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


    //public static IEnumerator FadeToDifferentMusic(float duration, float targetVolume, AudioClip targetMusic)
    //{
    //    float currentTime = 0;
    //    float start = audioSource.volume;

    //    while (currentTime < duration)
    //    {
    //        currentTime += Time.deltaTime;
    //        audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
    //        yield return null;
    //    }
    //    yield break;
    //}
}
