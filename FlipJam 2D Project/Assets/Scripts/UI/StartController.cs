using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StartController : MonoBehaviour
{

    [SerializeField] private GameObject textObj;
    [SerializeField] private float blinkTime = 1f;
    [SerializeField] private String firstScene;
    float blinkCounter = 0f;
    [SerializeField] private VideoClip gameplayVideo;

    // Start is called before the first frame update
    void Start()
    {
        GameController.instance.UnPauseGame();
        MusicManager.Instance.StopAllMusic();

        // Will attach a VideoPlayer to the main camera.
        GameObject camera = GameObject.Find("Main Camera");

        // VideoPlayer automatically targets the camera backplane when it is added
        // to a camera object, no need to change videoPlayer.targetCamera.
        var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();

        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = false;

        // By default, VideoPlayers added to a camera will use the far plane.
        // Let's target the near plane instead.
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        // Set the video to play. URL supports local absolute or relative paths.
        // Here, using absolute.
        videoPlayer.clip = gameplayVideo;

        // Skip the first 100 frames.
        //videoPlayer.frame = 100;

        // Restart from beginning when done.
        videoPlayer.isLooping = true;

        // Start playback. This means the VideoPlayer may have to prepare (reserve
        // resources, pre-load a few frames, etc.). To better control the delays
        // associated with this preparation one can use videoPlayer.Prepare() along with
        // its prepareCompleted event.
        videoPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            GameController.instance.OnSceneChange(firstScene);
        }

        if (blinkCounter >= blinkTime)
        {
            textObj.SetActive(!textObj.activeSelf);
            blinkCounter = 0f;
        }
        else
        {
            blinkCounter += Time.deltaTime;
        }
    }
}
