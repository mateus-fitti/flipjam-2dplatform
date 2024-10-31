using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    private GameObject player;
    private GameObject playerTwo;

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
            HeatMeasurementSystem heatSystem = FindObjectOfType<HeatMeasurementSystem>();
            if (heatSystem != null)
            {
                heatSystem.ResetHeatSystem();
            }
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
            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");

            GameObject virtualCam = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;

            if (GameController.instance.multiplayer)
            {
                if (GameController.instance.player2 < 0)
                {
                    GameController.instance.player2 = 0;
                }
                playerTwo = characters[GameController.instance.player2];
                playerTwo = Instantiate(playerTwo, spawnPosition.position + new Vector3(-1f, 1f, 0f), Quaternion.identity);
                playerTwo.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player2");
                Debug.Log("Player Two Spawned");
            }
        }

        if (gameMode == GameMode.Arena)
        {
            MusicManager.Instance.StopAndPlayMusic("ArenaMusic");
        }
        else
        {
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
        StartCoroutine(PlayDeadAnimationOnAllCharacters());
    }

    IEnumerator PlayDeadAnimationOnAllCharacters()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject character in characters)
        {
            PlayerMovement characterMovement = character.GetComponent<PlayerMovement>();
            if (characterMovement != null)
            {
                Animator characterAnimator = character.GetComponent<Animator>();
                characterMovement.PlayDeadAnimation();  // Trigger the "Dead" animation

                // yield return new WaitUntil(() => characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f && !characterAnimator.IsInTransition(0));
                yield return new WaitForSeconds(0.6f); // Wait for 1 second before moving to the next character

            }
        }

        // After all characters' animations have finished, execute the following
        Debug.Log("Game Over! The egg is frozen!");
        gameOverObj.SetActive(true); // Show the Game Over screen
        EventSystem.current.SetSelectedGameObject(restartButton);
        GameController.instance.PauseGame(); // Pause the game
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
}