using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static GameController instance;
    public int player1 = 0;
    public int player2 = 0;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
    }

    private void Update()
    {
        
    }

    public void OnSceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1.0f;
    }

    public void CharacterSelection(int id)
    {
        player1 = id;
    }


}