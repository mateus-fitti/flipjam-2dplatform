using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Add this for TextMeshPro

public class GameController : MonoBehaviour
{

    public static GameController instance;
    public TextMeshProUGUI scoreText; // Changed to TextMeshProUGUI
    public TextMeshProUGUI highScoreText; // Changed to TextMeshProUGUI
    public GameObject restartButton; // Reference to the Restart Button
    private float startTime;
    private bool gameStarted = true;
    private float timer = 0f; // Timer variable
    public TextMeshProUGUI timerText; // Changed to TextMeshProUGUI


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
        StartGame();
    }

    public void StartGame()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        restartButton.SetActive(false); // Ensure the restart button is hidden at start
        highScoreText.transform.parent.gameObject.SetActive(false);
        startTime = Time.time;

        HeatMeasurementSystem heatSystem = FindObjectOfType<HeatMeasurementSystem>();
        if (heatSystem != null)
        {
            heatSystem.ResetHeatSystem();
        }
        gameStarted = true;
        timer = 0; // Reset timer at the start of the game

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(-5f, 1.5f, 0); // Set to desired start position, adjust as necessary
        }
    }

    public void FinishGame()
    {
        if (!gameStarted) return;

        gameStarted = false;
        int score = CalculateScore();
        scoreText.text = "Your Score: " + score;

        // Check and update high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore = score; // Update local highScore variable to reflect new high score
        }
        highScoreText.transform.parent.gameObject.SetActive(true);
        highScoreText.text = "High Score: " + highScore.ToString();

        restartButton.SetActive(true); // Show the restart button when game ends
        PauseGame();
    }

    public void GameOver()
    {
        if (!gameStarted) return;

        gameStarted = false;
        restartButton.SetActive(true); // Show the restart button when game ends
        PauseGame(); // Pause the game
                     // Optionally, add any additional game over logic here
    }

    private int CalculateScore()
    {
        float timeTaken = Time.time - startTime;
        // Assuming higher score for less time, adjust the formula as needed
        int score = Mathf.Max(0, (int)(1000 - timeTaken * 10));
        return score;
    }


    private void Update()
    {
        if (gameStarted)
        {
            // Increase the timer
            timer += Time.deltaTime;
            // Update the timer UI text
            timerText.text = timer.ToString("F2"); // Display time with 2 decimal places
            scoreText.text = "Score: " + CalculateScore();
        }
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

    public void RestartGame()
    {
        UnPauseGame(); // Ensure the game is not paused
        
        // Reset game state variables
        gameStarted = true;
        timer = 0f; // Reset timer
        startTime = Time.time; // Reset start time for score calculation

        // Optionally, reset score or other game state variables here
        // score = 0; // Assuming there's a score variable to reset

        // Update UI elements to reflect the reset state
        scoreText.text = "Score: 0";
        timerText.text = "0.00"; // Reset timer display
        restartButton.SetActive(false); // Hide the restart button

        // Call StartGame to reinitialize game start state
        StartGame();

    }

}