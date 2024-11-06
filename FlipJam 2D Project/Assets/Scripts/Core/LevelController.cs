using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.UI;
using UnityEditor;

public class LevelController : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Changed to TextMeshProUGUI
    public TextMeshProUGUI highScoreText; // Changed to TextMeshProUGUI
    public GameObject gameOverObj;
    public GameObject restartButton; // Reference to the Restart Button
    public GameObject victoryObj;
    public GameObject menuButton;
    private float startTime;
    private bool gameStarted = true;
    private float timer = 0f; // Timer variable
    public TextMeshProUGUI timerText; // Changed to TextMeshProUGUI
    public GameObject[] characters;
    public Transform spawnPosition;
    public Transform spawnPosition2;
    private GameObject player;
    private GameObject playerTwo;
    private PlayerInput p1;
    private PlayerInput p2;
    public VictoryScreenController victoryScreenController; // Reference to the VictoryScreenController

    public enum GameMode { Normal, Arena }
    public GameMode gameMode = GameMode.Normal; // Variable to define the game mode

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        GameController.instance.UnPauseGame(); // Use GameController to unpause
        gameOverObj.SetActive(false); // Hide the Game Over screen
        victoryObj.SetActive(false);

        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        highScoreText.transform.parent.gameObject.SetActive(false);
        startTime = Time.time;

        if (gameMode == GameMode.Normal)
        {
            GameObject thermometer = GameObject.Find("Temperature");
            if (thermometer != null)
            {
                HeatMeasurementSystem heatSystem = thermometer.GetComponent<HeatMeasurementSystem>();
                if (heatSystem != null)
                {
                    heatSystem.ResetHeatSystem();
                }
            }
        }

        // Find and disable the thermometer if the game mode is Arena
        GameObject thermometerArena = GameObject.Find("Temperature");
        if (gameMode == GameMode.Arena && thermometerArena != null)
        {
            thermometerArena.SetActive(false);
        }

        gameStarted = true;
        timer = 0; // Reset timer at the start of the game

        player = GameObject.FindGameObjectWithTag("Player");
        if (spawnPosition != null)
        {
            if (player != null)
            {
                Destroy(player);
            }

            if (GameController.instance.player1 < 0)
            {
                GameController.instance.player1 = 0;
            }
            player = characters[GameController.instance.player1];
            player = Instantiate(player, spawnPosition.position, Quaternion.identity);
            p1 = player.GetComponent<PlayerInput>();
            Debug.Log("Player One Spawned");

            if (GameController.instance.multiplayer)
            {
                if (GameController.instance.player2 < 0)
                {
                    GameController.instance.player2 = 0;
                }
                playerTwo = characters[GameController.instance.player2];
                playerTwo = Instantiate(playerTwo, spawnPosition.position + new Vector3(-1f, 1f, 0f), Quaternion.identity);
                p2 = playerTwo.GetComponent<PlayerInput>();
                Debug.Log("Player Two Spawned");
            }
            ConnectControllers();

            // Register callback for when a device is added or removed
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        if (gameMode == GameMode.Arena)
        {
            MusicManager.Instance.StopAndPlayMusic("ArenaMusic");
        }
        else
        {
            GameObject virtualCam = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
            MusicManager.Instance.StopAndPlayMusic("LevelMusic");
        }
    }

    private void Update()
    {
        if (gameStarted)
        {
            // Increase the timer
            timer += Time.deltaTime;
            // Convert timer to minutes and seconds
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer % 60F);

            // Update the timer UI text to show as "MM:SS"
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Update the score text
            scoreText.text = "Score: " + CalculateScore();

        }
        else
        {
            restartButton.SetActive(true);
        }

        // Dois botões para pausar, no teclado: Backspace e R
        // No flipe: Start e Botão de cima da direita (em teoria)
        if (player.GetComponent<PlayerInput>().actions["Pause"].WasPressedThisFrame())
        {
            SoundManager.Instance.PlaySound2D("Button", false);
            if (Time.timeScale > 0)
            {
                GameController.instance.PauseGame();
            }
            else
            {
                GameController.instance.UnPauseGame();
            }
        }

        // Se o jogo tiver pausado e a tecla de Cancel for pressionada, volta para o Menu
        if (player.GetComponent<PlayerInput>().actions["Reset"].WasPressedThisFrame() && Time.timeScale == 0)
        {
            SoundManager.Instance.PlaySound2D("Button", false);
            GameController.instance.OnSceneChange("StartScene");
        }

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
        /*if (Input.GetButtonDown("Debug Reset"))
        {
            GameController.instance.OnSceneChange(SceneManager.GetActiveScene().name);
        }*/
    }

    public void FinishGame()
    {
        if (!gameStarted) return;

        gameStarted = false;
        int score = CalculateScore();
        scoreText.text = "Score: " + score;

        // Check and update high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore = score; // Update local highScore variable to reflect new high score
        }
        highScoreText.transform.parent.gameObject.SetActive(true);
        highScoreText.text = "High Score: " + highScore.ToString();

        SoundManager.Instance.PlaySound2D("Victory", false);
    }

    private int CalculateScore()
    {
        float timeTaken = Time.time - startTime;
        // Assuming higher score for less time, adjust the formula as needed
        int score = Mathf.Max(0, (int)(4000 - timeTaken * 10));
        return score;
    }

    public void GameOver()
    {
        StartCoroutine(HandleGameOver());
    }

    private IEnumerator HandleGameOver()
    {
        SoundManager.Instance.PlaySound2D("Victory", false);

        // Wait for 1 second after the death animation finishes
        yield return new WaitForSeconds(1f);

        // Change to the CharacterSelection scene
        //GameController.instance.OnSceneChange("MenuScene");
        if (gameMode == GameMode.Arena)
        {
            // Determine the winner based on which player is still alive
            (string winner, int playerNumber) = DetermineWinner();
            victoryScreenController.ShowVictoryScreen(winner, playerNumber);
        }
        else
        {
            Debug.Log("Game Over! The egg is frozen!");
            gameOverObj.SetActive(true); // Show the Game Over screen
            EventSystem.current.SetSelectedGameObject(restartButton);
            GameController.instance.PauseGame();
        }
    }

    private (string, int) DetermineWinner()
    {
        if (player != null)
        {
            PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();
            if (playerInfo != null && playerInfo.currentHealth > 0)
            {
                CharacterItemInteractions characterItemInteractions = player.GetComponent<CharacterItemInteractions>();
                if (characterItemInteractions != null)
                {
                    return (characterItemInteractions.characterType.ToString(), playerInfo.playerNumber);
                }
            }
        }

        if (playerTwo != null)
        {
            PlayerInfo playerInfoTwo = playerTwo.GetComponent<PlayerInfo>();
            if (playerInfoTwo != null && playerInfoTwo.currentHealth > 0)
            {
                CharacterItemInteractions characterItemInteractions = playerTwo.GetComponent<CharacterItemInteractions>();
                if (characterItemInteractions != null)
                {
                    return (characterItemInteractions.characterType.ToString(), playerInfoTwo.playerNumber);
                }
            }
        }

        return ("Unknown", 0);
    }

    public void Victory()
    {
        FinishGame();
        Debug.Log("Game Won! The egg is safe!");
        victoryObj.SetActive(true); // Show the Game Over screen
        EventSystem.current.SetSelectedGameObject(menuButton);
        GameController.instance.PauseGame(); // Pause the game
    }

    public void OnSceneChange(string sceneName)
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.OnSceneChange(sceneName);
    }

    public void RestartGame()
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.OnSceneChange(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        SoundManager.Instance.PlaySound2D("Button", false);
        GameController.instance.ExitGame();
    }

    void ConnectControllers()
    {
        p1.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        p1.SwitchCurrentActionMap("Player");
        if (GameController.instance.multiplayer)
        {
            p2.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
            p2.SwitchCurrentActionMap("Player2");
        }

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
                    p2.SwitchCurrentActionMap("Player2");
                }
                else
                {
                    p1.SwitchCurrentControlScheme("Xbox Controller", controller);
                    p1.SwitchCurrentActionMap("Player");
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
                        p2.SwitchCurrentActionMap("PlayerNotFlipJam2");
                    }
                }
                else
                {
                    if (p1.currentControlScheme != "Xbox Controller")
                    {
                        p1.SwitchCurrentControlScheme("Gamepad", controller);
                        p1.SwitchCurrentActionMap("PlayerNotFlipJam");
                    }
                    d++;
                }
            }
        }
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