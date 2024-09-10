using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        AudioClip nextTrack = musicLibrary.GetClipFromName(trackName);
        // Check if the desired track is already playing
        if (musicSource.isPlaying && musicSource.clip == nextTrack)
        {
            return; // Exit the method if the same track is already playing
        }
        StartCoroutine(AnimateMusicCrossfade(nextTrack, fadeDuration));
    }

    public void StopAndPlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        // Stop any currently playing music
        musicSource.Stop();

        // Retrieve the AudioClip from the music library
        AudioClip trackToPlay = musicLibrary.GetClipFromName(trackName);

        StartCoroutine(AnimateMusicCrossfade(trackToPlay, fadeDuration));

    }

    public void StopAndContinueMusic(string trackName, float fadeDuration = 0.5f)
    {
        AudioClip trackToPlay = musicLibrary.GetClipFromName(trackName);

        // Check if the desired track is already playing
        if (musicSource.isPlaying && musicSource.clip == trackToPlay)
        {
            return; // The requested song is already playing, so do nothing
        }
        else
        {
            // Stop any currently playing music
            musicSource.Stop();

            // Start the fade effect to play the requested song
            StartCoroutine(AnimateMusicCrossfade(trackToPlay, fadeDuration));
        }
    }

    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float initialVolume = musicSource.volume; // Store the initial volume
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(initialVolume, 0, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, initialVolume, percent);
            yield return null;
        }
    }

    public void StopAllMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}