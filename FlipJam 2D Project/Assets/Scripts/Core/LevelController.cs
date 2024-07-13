using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;
using Cinemachine;

public class LevelController : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Changed to TextMeshProUGUI
    public TextMeshProUGUI highScoreText; // Changed to TextMeshProUGUI
    public GameObject restartButton; // Reference to the Restart Button
    private float startTime;
    private bool gameStarted = true;
    private float timer = 0f; // Timer variable
    public TextMeshProUGUI timerText; // Changed to TextMeshProUGUI
    public GameObject[] characters;
    public Transform spawnPosition;


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        //restartButton.SetActive(false); // Ensure the restart button is hidden at start
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
        if (spawnPosition != null)
        {
            if (player != null)
            {
                Destroy(player);
            }

            player = characters[GameController.instance.player1];
            player = Instantiate(player, spawnPosition.position, Quaternion.identity);
            GameObject virtualCam = GameObject.FindGameObjectWithTag("VirtualCamera");
            virtualCam.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
        }
        MusicManager.Instance.StopAndPlayMusic("LevelMusic");

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

        GameController.instance.PauseGame();
    }

    private int CalculateScore()
    {
        float timeTaken = Time.time - startTime;
        // Assuming higher score for less time, adjust the formula as needed
        int score = Mathf.Max(0, (int)(4000 - timeTaken * 10));
        return score;
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
    GameController.instance.OnSceneChange(sceneName);
}

public void RestartGame()
{
    GameController.instance.OnSceneChange(SceneManager.GetActiveScene().name);

}


}