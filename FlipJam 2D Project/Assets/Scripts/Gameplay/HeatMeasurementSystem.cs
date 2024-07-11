using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for IEnumerator

public class HeatMeasurementSystem : MonoBehaviour
{
    public GameObject egg; // Referência para o GameObject do Egg
    public GameObject GameOverObj; // Referência para o GameObject do GameOver
    public Slider temperatureSlider; // Reference to the UI Slider that acts as a temperature progress bar
    public float temperature = 100f; // Temperatura inicial

    // Cores para os estados do Egg
    public Sprite heatedEggImage;
    public Sprite balancedEggImage;
    public Sprite coldEggImage;
    public Sprite frozenEggImage;

    public bool heatSystemActive = false; // Flag para indicar se o sistema de calor está ativo

    private bool isGameOver = false; // Flag para indicar se o jogo acabou

    public float decreaseRate = 1f; // Taxa de diminuição da temperatura por segundo
    private Image sliderFill;
    private Animator characterAnimator; // Add this field

    void Awake()
    {
        isGameOver = false;
    }

    void Start()
    {
        characterAnimator = GameObject.Find("Character").GetComponent<Animator>();
        GameController.instance.UnPauseGame(); // Use GameController to unpause
        GameOverObj.SetActive(false); // Hide the Game Over screen

        if (temperatureSlider != null)
        {
            temperatureSlider.maxValue = 100f; // Assuming 100 is the max temperature
            temperatureSlider.value = temperature; // Set to initial temperature
        }
    }
    void Update()
    {
        if (heatSystemActive)
        {
            // Diminuir a temperatura com o tempo
            if (temperature > 0) temperature -= decreaseRate * Time.deltaTime;

            // Atualizar a cor do Egg com base na temperatura
            UpdateEggColor();

            // Atualizar o UI do termômetro
            UpdateThermometerUI();
        }

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
            egg.GetComponent<SpriteRenderer>().sprite = heatedEggImage;
            sliderFill.color = Color.red;
        }
        else if (temperature > 50)
        {
            egg.GetComponent<SpriteRenderer>().sprite = balancedEggImage;
            sliderFill.color = Color.yellow; // Change slider color to balancedColor
        }
        else if (temperature > 25)
        {
            egg.GetComponent<SpriteRenderer>().sprite = coldEggImage;
            sliderFill.color = Color.grey; // Change slider color to coldColor
        }
        else
        {
            egg.GetComponent<SpriteRenderer>().sprite = frozenEggImage;
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
        if (isGameOver) return;

        StartCoroutine(WaitForAnimation("Dead")); // Wait for the animation to finish before pausing
    }

    IEnumerator WaitForAnimation(string animationName)
    {
       // Play the animation
    characterAnimator.Play(animationName);

    // Wait until the animation starts
    while (!characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
    {
        yield return null;
    }

    // Wait for one frame to ensure the animation has started
    yield return null;

    // Then wait until the animation is no longer playing
    while (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
           characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    {
        yield return null;
    }

    isGameOver = true;
    Debug.Log("Game Over! The egg is frozen!");
    GameOverObj.SetActive(true);
    // Finally, pause the game
    GameController.instance.PauseGame();
}
}