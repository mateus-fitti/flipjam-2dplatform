using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("First Selected Options")]
    [SerializeField] private GameObject startFirst;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(startFirst);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSceneChange(String sceneName)
    {
        GameController.instance.OnSceneChange(sceneName);
    }
}
