using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Manager;
    public AudioSource MusicSource;
    public AudioSource SoundsSource;
    public AudioClip SelectAudio;
    public AudioClip MatchedAudio;
    public AudioClip HitAudio;
    public AudioClip BombSound;
    public AudioClip RocketSound;

    private void OnEnable()
    {
        Manager = GetComponent<AudioManager>();
        SoundsSource = Camera.main.GetComponent<AudioSource>();
    }

    public static void PlaySelectSound()
    {
        Manager.SoundsSource.PlayOneShot(Manager.SelectAudio);
    }

    public static void PlayMatchedSound()
    {
        Manager.SoundsSource.PlayOneShot(Manager.MatchedAudio);
    }

    public static void PlayHitSound()
    {
        Manager.SoundsSource.PlayOneShot(Manager.HitAudio);
    }

    public static void PlayBombSound()
    {
        Manager.SoundsSource.PlayOneShot(Manager.BombSound);
    }

    public static void PlayRocketSound()
    {
        Manager.SoundsSource.PlayOneShot(Manager.RocketSound);
    }

}
