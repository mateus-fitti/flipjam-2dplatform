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

    public void ShowVictoryScreen(string winner, int playerNumber)
    {
        if (winner == "Ngoro")
        {
            charImg.sprite = ngoroSprite;
        }
        else if (winner == "Aunfryn")
        {
            charImg.sprite = aunfrynSprite;
        }

        victoryText.text = $"Jogador {playerNumber} venceu!";

        gameObject.SetActive(true); // Show the victory screen
        GameController.instance.PauseGame(); // Pause the game
    }
}