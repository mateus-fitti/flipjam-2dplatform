using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{

    [SerializeField] private GameObject textObj;
    [SerializeField] private float blinkTime = 1f;
    [SerializeField] private String firstScene;
    float blinkCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
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
        } else {
            blinkCounter += Time.deltaTime;
        }
    }
}
