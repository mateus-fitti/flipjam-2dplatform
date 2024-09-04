using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CampusController : MonoBehaviour
{
    [SerializeField] private float mensageTime = 1f;
    float mensageCounter = 0f;

    [SerializeField] private GameObject nextText;
    [SerializeField] private float blinkTime = 1f;
    float blinkCounter = 0f;


    // Start is called before the first frame update
    void Start()
    {
        mensageCounter = 1f;
        GameController.instance.UnPauseGame();
    }

    void Update()
    {
        if (Input.anyKeyDown && mensageCounter <= 0f)
        {
            mensageCounter = mensageTime;
            SoundManager.Instance.PlaySound2D("Button", false);
            GameController.instance.OnSceneChange("LogoScene");
        }
        
        if (mensageCounter > 0f)
        {
            nextText.SetActive(false);
        }
        else
        {
            if (blinkCounter >= blinkTime)
            {
                nextText.SetActive(!nextText.activeSelf);
                blinkCounter = 0f;
            }
        }

        mensageCounter -= Time.deltaTime;
        blinkCounter += Time.deltaTime;
    }

}
