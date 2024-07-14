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
        MusicManager.Instance.StopAndContinueMusic("MenuMusic");
        EventSystem.current.SetSelectedGameObject(startFirst);

        GameController.instance.UnPauseGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSceneChange(String sceneName)
    {
        GameController.instance.OnSceneChange(sceneName);
    }

    public void CharacterSelection(int character_id)
    {
        GameController.instance.CharacterSelection(character_id);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartButton"));
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        GameController.instance.ExitGame();
    }
}
