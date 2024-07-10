using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // IMPRIME A TECLA OU BOTÃO PRESSIONADO
        /*
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log(keyCode);
            }
        }
        */

        // Restart game - PARA TESTES
        if (Input.GetButtonDown("Debug Reset"))
        {
            GameController.instance.OnSceneChange(SceneManager.GetActiveScene().name);
        }
    }
}
