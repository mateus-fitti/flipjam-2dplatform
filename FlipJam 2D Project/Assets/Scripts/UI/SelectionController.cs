using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    private PlayerInputManager playerIManager;
    public GameObject playerUIPrefab;
    public GameObject[] p1Icon;
    public GameObject[] p2Icon;
    public GameObject[] p1charsIcons;
    public GameObject[] p2charsIcons;
    public GameObject player2Canvas;
    private PlayerInput p1;
    private PlayerInput p2;
    private bool player1Selected = false;

    void Awake()
    {
        playerIManager = GetComponent<PlayerInputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.Instance.StopAndContinueMusic("MenuMusic");
        GameController.instance.UnPauseGame();

        p1 = GameObject.Find("Player1UI").GetComponent<PlayerInput>();
        p2 = GameObject.Find("Player2UI").GetComponent<PlayerInput>();

        ConnectControllers();

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadArenaMap()
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.LoadRandomArenaMap();
    }

    public void PlayerReady()
    {
        player2Canvas.SetActive(true);
    }

    public void SetCharacter(int player, int character_id)
    {
        if (GameController.instance.multiplayer)
        {
            if (!player1Selected)
            {
                GameController.instance.player1 = character_id;
                player1Selected = true;
                AssignPlayerInfo(character_id, 1);
            }
            else
            {
                GameController.instance.player2 = character_id;
                AssignPlayerInfo(character_id, 2);
            }
        }
        else
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
    }

    private void AssignPlayerInfo(int character_id, int playerNumber)
    {
        GameObject characterPrefab = characterPrefabs[character_id];
        GameObject characterInstance = Instantiate(characterPrefab);
        PlayerInfo playerInfo = characterInstance.GetComponent<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.SetPlayerNumber(playerNumber);
        }
    }

    public void CancelAction()
    {
        player2Canvas.SetActive(false);
        player1Selected = false;
    }

    public void ButtonSound()
    {
        SoundManager.Instance.PlaySound2D("Button", false);
    }

    void ConnectControllers()
    {
        p1.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        p1.SwitchCurrentActionMap("UI");
        p2.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        p2.SwitchCurrentActionMap("UI2");

        int c = 0;
        int d = 0;

        foreach (InputDevice controller in InputSystem.devices)
        {
            Debug.Log("DEVICE: " + controller.name);
            if (controller is XInputController)
            {
                if (c > 0 && GameController.instance.multiplayer)
                {
                    p2.SwitchCurrentControlScheme("Xbox Controller", controller);
                    p2.SwitchCurrentActionMap("UI2");
                }
                else
                {
                    p1.SwitchCurrentControlScheme("Xbox Controller", controller);
                    p1.SwitchCurrentActionMap("UI");
                    c++;
                }
            }
            else if (controller is Gamepad)
            {
                if (d > 0 && GameController.instance.multiplayer)
                {
                    if (p2.currentControlScheme != "Xbox Controller")
                    {
                        p2.SwitchCurrentControlScheme("Gamepad", controller);
                        p2.SwitchCurrentActionMap("UINotFlipJam2");
                    }
                }
                else
                {
                    if (p1.currentControlScheme != "Xbox Controller")
                    {
                        p1.SwitchCurrentControlScheme("Gamepad", controller);
                        p1.SwitchCurrentActionMap("UINotFlipJam");
                    }
                    d++;
                }
            }
        }

        SetUIActions(p1.GetComponent<InputSystemUIInputModule>(), p1.currentActionMap);
        SetUIActions(p2.GetComponent<InputSystemUIInputModule>(), p2.currentActionMap);
    }

    void SetUIActions(InputSystemUIInputModule uiInputModule, InputActionMap actionMap)
    {
        uiInputModule.move = InputActionReference.Create(actionMap.FindAction("Navigate"));
        uiInputModule.submit = InputActionReference.Create(actionMap.FindAction("Submit"));
        uiInputModule.cancel = InputActionReference.Create(actionMap.FindAction("Cancel"));
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        ConnectControllers();
    }

    // Unregister callback when the object is destroyed to avoid memory leaks
    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}