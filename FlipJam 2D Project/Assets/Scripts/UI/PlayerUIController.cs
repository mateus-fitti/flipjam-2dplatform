using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{

    [SerializeField] private int id;
    private PlayerInput uiInput;
    private MultiplayerEventSystem mEventSystem;
    public GameObject firstObject;
    public GameObject readyButton;
    public SelectionController selectionController;
    private bool charSelected;

    void Awake()
    {
        uiInput = GetComponent<PlayerInput>();
        mEventSystem = GetComponent<MultiplayerEventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //iInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();

        //DEPENDENDO DO DISPOSITIVO DE ENTRADA PODE SER NECESSÁRIO ALTERAR O ACTION MAP, MAS ISSO NÃO ALTERA OS INPUTS DE UI
        /*if (uiInput.devices[0] is not XInputController)
        {
            uiInput.SwitchCurrentActionMap("UINotFlipJam");
            Debug.Log("Current Action Map is " + uiInput.currentActionMap);
            Debug.Log("Current Player " + id + " device is " + uiInput.devices[0]);
        }*/
        mEventSystem.SetSelectedGameObject(firstObject);
        charSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mEventSystem.currentSelectedGameObject == null && !charSelected)
        {
            mEventSystem.SetSelectedGameObject(firstObject);
        }

        // For debug
        /*if(uiInput.actions["Submit"].WasPerformedThisFrame())
        {
            Debug.Log("Player " + id + " pressed Start!");
        }*/
    }

    public void SelectMyCharacter(int characterId)
    {
        selectionController.SetCharacter(id, characterId);
        mEventSystem.SetSelectedGameObject(readyButton);
    }

    public void CharacterSelected()
    {
        charSelected = true;
        mEventSystem.SetSelectedGameObject(null);
    }

    void OnCancel()
    {
        charSelected = false;

        Button bt = readyButton.GetComponent<Button>();

        if (id == 1 || (id == 2 && !bt.interactable))
        {
            selectionController.CancelAction();
        }

        mEventSystem.SetSelectedGameObject(firstObject);
        bt.interactable = false;
    }
}
