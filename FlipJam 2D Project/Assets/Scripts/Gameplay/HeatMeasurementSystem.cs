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
    public Sprite[] temperatureStateSprites; // Array of sprites for each temperature state
    public LayerMask heatLayer;

    public bool heatSystemActive = false; // Flag para indicar se o sistema de calor está ativo

    private bool isGameOver = false; // Flag para indicar se o jogo acabou

    private float decreaseRate;
    private float reducedDreceasedRate;
    public float defaultDecreasedRate = 2f; // Taxa de diminuição da temperatura por segundo
    public float defaultIncreasedRate = 6f; // Taxa de ganho da temperatura por segundo
    private Image sliderFill;

    void Awake()
    {
        isGameOver = false;
    }

    void Start()
    {
        GameController.instance.UnPauseGame(); // Use GameController to unpause
        GameOverObj.SetActive(false); // Hide the Game Over screen

        decreaseRate = defaultDecreasedRate;
        reducedDreceasedRate = defaultDecreasedRate / 2;

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
            Collider2D collider = Physics2D.OverlapCircle(egg.transform.position, 1, heatLayer);

            if (collider == null)
            {
                // Diminuir a temperatura com o tempo
                if (!egg.GetComponent<SpriteRenderer>().enabled)
                {
                    decreaseRate = reducedDreceasedRate;
                    if (temperature > 0) temperature -= decreaseRate * Time.deltaTime;
                }
                else
                {
                    decreaseRate = defaultDecreasedRate;
                    if (temperature > 0) temperature -= decreaseRate * Time.deltaTime;
                }
            }
            else
            {
                if (temperature > 0 && temperature < 100) temperature += defaultIncreasedRate * Time.deltaTime;
            }
            // Atualizar a cor do Egg com base na temperatura
            UpdateEggColor();

            // Atualizar o UI do termômetro
            UpdateThermometerUI();
        }

    }

    void UpdateEggColor()
    {
        UpdateThermometerUI(); // Update the thermometer UI

        // Update the egg color and slider fill color based on the temperature
        // This part remains for compatibility with other game logic, might be adjusted or removed depending on specific needs
        if (temperature > 75)
        {
            egg.GetComponent<SpriteRenderer>().sprite = heatedEggImage;
        }
        else if (temperature > 50)
        {
            egg.GetComponent<SpriteRenderer>().sprite = balancedEggImage;
        }
        else if (temperature > 25)
        {
            egg.GetComponent<SpriteRenderer>().sprite = coldEggImage;
        }
        else
        {
            egg.GetComponent<SpriteRenderer>().sprite = frozenEggImage;
            GameOver(); // Trigger Game Over when temperature is 25 or below
        }
    }

    void UpdateThermometerUI()
    {
        if (sliderFill == null)
        {
            // Assuming the Slider's Fill GameObject is named "Fill", and it's the first child of the second child of the Slider
            sliderFill = temperatureSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        }

        // Calculate the index for the sprite based on the temperature
        int spriteIndex = Mathf.FloorToInt((100f - temperature) / 10f);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, 8);

        Debug.Log("Sprite Index: " + spriteIndex);

        // Update the slider background sprite
        // Assuming the background child GameObject is named "Background" and it's the first child of the slider
        Image background = temperatureSlider.transform.GetChild(0).GetComponent<Image>();
        background.sprite = temperatureStateSprites[spriteIndex];
    }

    public void ResetHeatSystem()
    {
        temperature = 100f; // Reset to initial temperature
        heatSystemActive = true; // Reactivate the heat system
        isGameOver = false; // Reset game over flag
        GameOverObj.SetActive(false); // Hide the Game Over screen

        // Reset UI elements
        if (temperatureSlider != null)
        {
            temperatureSlider.value = temperature;
        }
        // Reset egg color and slider fill color
        UpdateEggColor();
    }
    void GameOver()
    {
        if (isGameOver) return;

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
        isGameOver = true;
        Debug.Log("Game Over! The egg is frozen!");
        GameOverObj.SetActive(true); // Show the Game Over screen
        GameController.instance.GameOver();
    }
}