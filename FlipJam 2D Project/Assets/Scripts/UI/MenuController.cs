using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MenuController : MonoBehaviour
{
    [Header("First Selected Options")]
    [SerializeField] private GameObject startFirst;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.Instance.StopAndContinueMusic("MenuMusic");
        EventSystem.current.SetSelectedGameObject(startFirst);
        foreach (MultiplayerEventSystem eventSystem in FindObjectsOfType(typeof(MultiplayerEventSystem)))
        {
            eventSystem.SetSelectedGameObject(startFirst);
        }

        GameController.instance.UnPauseGame();

        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerUI");
        if (!GameController.instance.multiplayer)
        {
            foreach (GameObject player in players)
            {
                if (player.name != "PlayerUI")
                {
                    GameObject.Destroy(player);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSceneChange(String sceneName)
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.OnSceneChange(sceneName);
    }

    public void CharacterSelection(int character_id)
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.CharacterSelection(character_id);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartButton"));
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.ExitGame();
    }

    public void IsMultiplayer(bool mult)
    {
        GameController.instance.multiplayer = mult;
    }
}
