using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject[] characters;
    private PlayerInputManager playerIManager;
    public GameObject playerUIPrefab;
    public GameObject[] p1Icon;
    public GameObject[] p2Icon;
    public GameObject[] charsIcons;

    void Awake()
    {
        playerIManager = GetComponent<PlayerInputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.instance.multiplayer)
        {
            gameObject.SetActive(false);
        }
        /*if (GameController.instance.multiplayer)
        {
            GameObject p2UI = Instantiate(playerUIPrefab);
            
            p2UI.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(GameObject.Find("Card2Button"));
            p2UI.GetComponent<MultiplayerEventSystem>().playerRoot = GameObject.Find("Canvas");

            p2UI.GetComponent<PlayerInput>().enabled = true;
            p2UI.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI2");
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectCharacter(int character_id)
    {
        if (GameController.instance.multiplayer)
        {
            charsIcons[0].SetActive(true);
            charsIcons[1].SetActive(true);

            if (character_id == 0)
            {
                GameController.instance.player1 = 0;
                GameController.instance.player2 = 1;

                p1Icon[0].SetActive(true);
                p2Icon[0].SetActive(false);
                p1Icon[1].SetActive(false);
                p2Icon[1].SetActive(true);
            } else {
                GameController.instance.player1 = 1;
                GameController.instance.player2 = 0;

                p1Icon[0].SetActive(false);
                p2Icon[0].SetActive(true);
                p1Icon[1].SetActive(true);
                p2Icon[1].SetActive(false);
            }
            if (GameController.instance.player1 > 0 && GameController.instance.player2 > 0)
            {
                // HABILITAR START
            }
        }
    }
}
