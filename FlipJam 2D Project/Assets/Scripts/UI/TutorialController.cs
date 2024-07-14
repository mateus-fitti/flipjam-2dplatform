using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private float mensageTime = 1f;
    float mensageCounter = 0f;

    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject[] tutorialPanels;
    int panelCounter = 0;
    [SerializeField] private String nextScene;

    [SerializeField] private float blinkTime = 1f;
    float blinkCounter = 0f;



    void Start()
    {
        mensageCounter = 1f;
        GameController.instance.UnPauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && mensageCounter <= 0f)
        {
            mensageCounter = mensageTime;
            if (panelCounter < tutorialPanels.Length - 1)
            {
                tutorialPanels[panelCounter].SetActive(false);
                panelCounter++;
                tutorialPanels[panelCounter].SetActive(true);
            }
            else
            {
                GameController.instance.OnSceneChange(nextScene);
            }
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


/*
if (Input.anyKeyDown && mensageCounter <= 0f)
        {
            mensageCounter = mensageTime;
            nextText.SetActive(true);
            skipText.SetActive(true);
        }

        if (mensageCounter > 0f)
        {
            if (mensageCounter < mensageTime - .1f)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (panelCounter < tutorialPanels.Length)
                    {
                        tutorialPanels[panelCounter].SetActive(true);
                        panelCounter++;
                    }
                    else
                    {
                        GameController.instance.OnSceneChange(nextScene);
                    }
                }
                if (Input.GetButtonDown("Fire1"))
                {
                    GameController.instance.OnSceneChange(nextScene);
                }
            }
        }
        else
        {
            nextText.SetActive(false);
            skipText.SetActive(false);
        }
*/