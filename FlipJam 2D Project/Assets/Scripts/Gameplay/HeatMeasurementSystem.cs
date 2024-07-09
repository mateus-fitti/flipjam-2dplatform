using UnityEngine;
using UnityEngine.UI;

public class HeatMeasurementSystem : MonoBehaviour
{
    public GameObject egg; // Referência para o GameObject do Egg
    public GameObject GameOverObj; // Referência para o GameObject do GameOver
    public Slider temperatureSlider; // Reference to the UI Slider that acts as a temperature progress bar
    public float temperature = 100f; // Temperatura inicial

    // Cores para os estados do Egg
    public Color heatedColor = Color.red;
    public Color balancedColor = Color.yellow;
    public Color coldColor = Color.blue;
    public Color frozenColor = Color.cyan;

    private bool isGameOver = false; // Flag para indicar se o jogo acabou

    public float decreaseRate = 1f; // Taxa de diminuição da temperatura por segundo

    private Image sliderFill;

    void Awake()
    {
        isGameOver = false;
    }

    void Start()
    {
        Time.timeScale = 1; // Unpause the game
        GameOverObj.SetActive(false); // Hide the Game Over screen

        if (temperatureSlider != null)
        {
            temperatureSlider.maxValue = 100f; // Assuming 100 is the max temperature
            temperatureSlider.value = temperature; // Set to initial temperature
        }
    }
    void Update()
    {
        // Diminuir a temperatura com o tempo
        if (temperature > 0) temperature -= decreaseRate * Time.deltaTime;

        // Atualizar a cor do Egg com base na temperatura
        UpdateEggColor();

        // Atualizar o UI do termômetro
        UpdateThermometerUI();
    }

    void UpdateEggColor()
    {
        // Dynamically find the Slider's Fill component if not already cached
        if (sliderFill == null)
        {
            // Assuming the Slider's Fill GameObject is named "Fill", and it's the first child of the second child of the Slider
            sliderFill = temperatureSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        }

        // Update the egg color and slider fill color based on the temperature
        if (temperature > 75)
        {
            egg.GetComponent<Renderer>().material.color = heatedColor;
            sliderFill.color = Color.red;
        }
        else if (temperature > 50)
        {
            egg.GetComponent<Renderer>().material.color = balancedColor;
            sliderFill.color = Color.yellow; // Change slider color to balancedColor
        }
        else if (temperature > 25)
        {
            egg.GetComponent<Renderer>().material.color = coldColor;
            sliderFill.color = Color.blue; // Change slider color to coldColor
        }
        else
        {
            egg.GetComponent<Renderer>().material.color = frozenColor;
            sliderFill.color = Color.cyan; // Change slider color to frozenColor
            GameOver(); // Trigger Game Over when temperature is 25 or below
        }
    }

    void UpdateThermometerUI()
    {
        if (temperatureSlider != null)
        {
            temperatureSlider.value = temperature;
        }
    }
    void GameOver()
    {
        if (isGameOver) return; // Return if the game is already over (to avoid calling this method multiple times
        isGameOver = true;
        Debug.Log("Game Over! The egg is frozen!");
        GameOverObj.SetActive(true); // Show the Game Over screen
        Time.timeScale = 0; // Pause the game
    }
}