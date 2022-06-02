using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _audioManager;
    public AudioSource MusicSource;
    public AudioSource SoundsSource;
    public AudioClip SelectAudio;
    public AudioClip MatchedAudio;
    public AudioClip HitAudio;
    public AudioClip BombSound;
    public AudioClip RocketSound;

    private void OnEnable()
    {
        _audioManager = GetComponent<AudioManager>();
        SoundsSource = GetComponent<AudioSource>();
    }

    public static void PlaySelectSound()
    {
        _audioManager.SoundsSource.PlayOneShot(_audioManager.SelectAudio);
    }

    public static void PlayMatchedSound()
    {
        _audioManager.SoundsSource.PlayOneShot(_audioManager.MatchedAudio);
    }

    public static void PlayHitSound()
    {
        _audioManager.SoundsSource.PlayOneShot(_audioManager.HitAudio);
    }

    public static void PlayBombSound()
    {
        _audioManager.SoundsSource.PlayOneShot(_audioManager.BombSound);
    }

    public static void PlayRocketSound()
    {
        _audioManager.SoundsSource.PlayOneShot(_audioManager.RocketSound);
    }

}
