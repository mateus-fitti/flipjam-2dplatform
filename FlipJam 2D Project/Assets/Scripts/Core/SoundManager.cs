using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;
    private AudioClip currentlyPlayingClip;
    private bool isClipPlaying = false;

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

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName, bool solo = true)
    {
        if (solo)
        {
            AudioClip clipToPlay = sfxLibrary.GetClipFromName(soundName);
            if (clipToPlay != null && (!isClipPlaying || currentlyPlayingClip != clipToPlay))
            {
                sfx2DSource.PlayOneShot(clipToPlay);
                currentlyPlayingClip = clipToPlay;
                isClipPlaying = true;
                StartCoroutine(ResetIsPlayingAfterClipEnds(clipToPlay.length));
            }
        } else
        {
            sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
        }

    }

    private IEnumerator ResetIsPlayingAfterClipEnds(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        isClipPlaying = false;
    }
}