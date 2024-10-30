using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    private PlayerInputManager playerIManager;
    public GameObject playerUIPrefab;
    public GameObject[] p1Icon;
    public GameObject[] p2Icon;
    public GameObject[] p1charsIcons;
    public GameObject[] p2charsIcons;
    public GameObject player2Canvas;

    void Awake()
    {
        playerIManager = GetComponent<PlayerInputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (GameController.instance.multiplayer)
        {
            gameObject.SetActive(false);
        }*/
        /*if (GameController.instance.multiplayer)
        {
            GameObject p2UI = Instantiate(playerUIPrefab);
            
            p2UI.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(GameObject.Find("Card2Button"));
            p2UI.GetComponent<MultiplayerEventSystem>().playerRoot = GameObject.Find("Canvas");

            p2UI.GetComponent<PlayerInput>().enabled = true;
            p2UI.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI2");
        }*/

        MusicManager.Instance.StopAndContinueMusic("MenuMusic");
        GameController.instance.UnPauseGame();
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

    public void PlayerReady()
    {
        player2Canvas.SetActive(true);
    }

    public void SetCharacter(int player, int character_id)
    {
        if (player == 1)
        {
            GameController.instance.player1 = character_id;
        }
        else
        {
            GameController.instance.player2 = character_id;
        }
    }

    public void CancelAction()
    {
        player2Canvas.SetActive(false);
    }
}
