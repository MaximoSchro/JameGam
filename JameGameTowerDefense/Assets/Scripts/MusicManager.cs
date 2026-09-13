using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static Action StartPrep;
    public static Action StartWave;
    [SerializeField] private AudioSource PrepMusicSource;
    [SerializeField] private AudioSource WaveMusicSource;
    private float maxVolume;
    private void OnEnable()
    {
        PrepMusicSource.Play();
        StartPrep += PlayPrep;
        StartWave += PlayWave;
    }
    private void OnDisable()
    {
        StartPrep -= PlayPrep;
        StartWave -= PlayWave;
        PrepMusicSource.Stop();
        WaveMusicSource.Stop();
    }
    private void PlayPrep()
    {
        StartCoroutine(SwitchTrack(WaveMusicSource, PrepMusicSource));
    }
    private void PlayWave()
    {
        StartCoroutine(SwitchTrack(PrepMusicSource, WaveMusicSource));
    }
    private IEnumerator SwitchTrack(AudioSource oldSource, AudioSource newSource)
    {
        while(oldSource.volume > 0.1f)
        {
            oldSource.volume = Mathf.Lerp(oldSource.volume, 0.0f, Time.deltaTime * 10);
            yield return null;
        }
        oldSource.volume = 0;
        oldSource.Stop();
        newSource.Play();
        newSource.volume = 0;
        while (newSource.volume < .7f)
        {
            newSource.volume = Mathf.Lerp(newSource.volume, 0.75f, Time.deltaTime * 10);
            yield return null;
        }
        newSource.volume = 0.75f;
    }
}
