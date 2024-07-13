using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro; // Add this for TextMeshPro

public class LevelController : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Changed to TextMeshProUGUI
    public TextMeshProUGUI highScoreText; // Changed to TextMeshProUGUI
    public GameObject restartButton; // Reference to the Restart Button
    private float startTime;
    private bool gameStarted = true;
    private float timer = 0f; // Timer variable
    public TextMeshProUGUI timerText; // Changed to TextMeshProUGUI


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
        GameController.instance.OnSceneChange(SceneManager.GetActiveScene().name);

    }


}