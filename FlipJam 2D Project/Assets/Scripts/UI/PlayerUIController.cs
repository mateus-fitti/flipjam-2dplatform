using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerUIController : MonoBehaviour
{

    private PlayerInput uiInput;

    void Awake()
    {
        uiInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //iInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiInput.actions["Submit"].WasPerformedThisFrame())
        {
            Debug.Log("PLAYER " + uiInput.playerIndex);
        }
    }
}
