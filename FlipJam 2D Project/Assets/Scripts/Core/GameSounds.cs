using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayLevelMusic()
    {
        MusicManager.Instance.StopAndPlayMusic("LevelMusic");
    }

    public void PlayMenuMusic()
    {
        MusicManager.Instance.StopAndPlayMusic("MenuMusic");
    }

    public void PlayButtonSound()
    {
        SoundManager.Instance.PlaySound2D("Button", false);
    }
}
