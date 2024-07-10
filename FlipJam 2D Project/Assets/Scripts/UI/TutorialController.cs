using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private float mensageTime = 2f;
    float mensageCounter = 0f;

    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject skipText;

    [SerializeField] private GameObject[] tutorialPanels;
    int panelCounter = 1;
    [SerializeField] private String nextScene;


    // Update is called once per frame
    void Update()
    {
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
                if (Input.GetButtonDown("Fire1"))
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
                if (Input.GetButtonDown("Fire2"))
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

        mensageCounter -= Time.deltaTime;
    }
}
