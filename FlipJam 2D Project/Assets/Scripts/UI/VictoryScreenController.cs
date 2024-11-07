using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenController : MonoBehaviour
{
    public Image charImg; // Reference to the Image component for the character
    public TextMeshProUGUI victoryText; // Reference to the TextMeshProUGUI component for the victory text
    public Sprite ngoroSprite; // Sprite for Ngoro
    public Sprite aunfrynSprite; // Sprite for Aunfryn
    private Button menuButton; // Reference to the Menu Button
    private Button rematchButton; // Reference to the Rematch Button

    void Awake()
    {
        
    }

    public void ShowVictoryScreen(string winner, int playerNumber)
    {
        // Find the buttons in the children
        menuButton = transform.Find("ArenaMenuButton").GetComponent<Button>();
        rematchButton = transform.Find("ArenaRematchButton").GetComponent<Button>();
        if (winner == "Ngoro")
        {
            charImg.sprite = ngoroSprite;
        }
        else if (winner == "Aunfryn")
        {
            charImg.sprite = aunfrynSprite;
        }

        victoryText.text = $"Jogador {playerNumber} venceu!";

        // Find the AudioManager instance
        GameObject audioManager = GameObject.Find("AudioManager");
        GameSounds gameSounds = null;
        if (audioManager != null)
        {
            gameSounds = audioManager.GetComponent<GameSounds>();
        }

        // Add OnClick events to the buttons
        menuButton.onClick.RemoveAllListeners();
        if (gameSounds != null)
        {
            menuButton.onClick.AddListener(() => gameSounds.PlayButtonSound());
        }
        menuButton.onClick.AddListener(() => GameController.instance.OnSceneChange("MenuScene"));

        rematchButton.onClick.RemoveAllListeners();
        if (gameSounds != null)
        {
            rematchButton.onClick.AddListener(() => gameSounds.PlayButtonSound());
        }
        rematchButton.onClick.AddListener(() => GameController.instance.OnSceneChange("CharMultiplayerSelection"));

        gameObject.SetActive(true); // Show the victory screen
        GameController.instance.PauseGame(); // Pause the game
    }
}