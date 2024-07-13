using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems; // Required for IEnumerator

public class HeatMeasurementSystem : MonoBehaviour
{
    public GameObject egg; // Referência para o GameObject do Egg
    public GameObject GameOverObj; // Referência para o GameObject do GameOver
    public GameObject temperatureImg; // Reference to the UI Image that acts as a temperature progress bar
    public float temperature = 100f; // Temperatura inicial

    // Cores para os estados do Egg
    public Sprite heatedEggImage;
    public Sprite balancedEggImage;
    public Sprite coldEggImage;
    public Sprite frozenEggImage;
    public Sprite[] temperatureStateSprites; // Array of sprites for each temperature state
    public LayerMask heatLayer;
    public LayerMask coldLayer;

    public bool heatSystemActive = false; // Flag para indicar se o sistema de calor está ativo

    private bool isGameOver = false; // Flag para indicar se o jogo acabou

    private float decreaseRate;
    private float reducedDreceasedRate;
    private float aumentedDecreasedRate;
    public float defaultDecreasedRate = 2f; // Taxa de diminuição da temperatura por segundo
    public float defaultIncreasedRate = 6f; // Taxa de ganho da temperatura por segundo

    private float lastSpriteChangeTemperature; // Track the last temperature at which the sprite was changed

    void Awake()
    {
        isGameOver = false;
        lastSpriteChangeTemperature = temperature;
    }

    void Start()
    {
        GameController.instance.UnPauseGame(); // Use GameController to unpause
        GameOverObj.SetActive(false); // Hide the Game Over screen

        decreaseRate = defaultDecreasedRate;
        reducedDreceasedRate = defaultDecreasedRate / 2;
        aumentedDecreasedRate = defaultDecreasedRate * 2;

    }
    void Update()
    {
        if (heatSystemActive)
        {
            Collider2D heatCollider = Physics2D.OverlapCircle(egg.transform.position, 1, heatLayer);
            Collider2D coldCollider = Physics2D.OverlapCircle(egg.transform.position, 1, coldLayer);

            if (heatCollider == null && coldCollider == null)
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
            else if(heatCollider != null)
            {
                if (temperature > 0 && temperature < 100) temperature += defaultIncreasedRate * Time.deltaTime;

            }
            else if(coldCollider != null)
            {
                Debug.Log("Esfriando: " + temperature);
                decreaseRate = aumentedDecreasedRate;
                    if (temperature > 0) temperature -= decreaseRate * Time.deltaTime;
            }
            // Atualizar a cor do Egg com base na temperatura
            UpdateEggColor();

            // Atualizar o UI do termômetro
            UpdateThermometerUI();
        }

    }

    void UpdateEggColor()
    {

        // Update the egg color and Image fill color based on the temperature
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
        // Determine the direction of temperature change
        bool isTemperatureIncreasing = temperature > lastSpriteChangeTemperature;

        // Calculate the difference in temperature since the last sprite change
        float temperatureDifference = Mathf.Abs(temperature - lastSpriteChangeTemperature);

        // Check if the temperature has changed by 6.5 degrees or more
        if (temperatureDifference >= 6f)
        {
            // Calculate how many steps of 6.5f fit into the temperature difference
            int steps = Mathf.FloorToInt(temperatureDifference / 6f);

            // Calculate the current index based on the direction of temperature change
            int currentIndex = Array.IndexOf(temperatureStateSprites, temperatureImg.GetComponent<Image>().sprite);
            int spriteIndex = currentIndex;

            if (isTemperatureIncreasing)
            {
                // Move backwards through the sprite array for increasing temperature
                spriteIndex -= steps;
            }
            else
            {
                // Move forwards through the sprite array for decreasing temperature
                spriteIndex += steps;
            }

            // Ensure the sprite index wraps around the array bounds
            if (spriteIndex < 0)
            {
                spriteIndex = temperatureStateSprites.Length - 1 + ((spriteIndex + 1) % temperatureStateSprites.Length);
            }
            else if (spriteIndex >= temperatureStateSprites.Length)
            {
                spriteIndex %= temperatureStateSprites.Length;
            }

            // Update the UI image sprite
            Image background = temperatureImg.GetComponent<Image>();
            background.sprite = temperatureStateSprites[spriteIndex];

            // Update the last temperature at which the sprite was changed
            lastSpriteChangeTemperature = temperature;
        }
    }
    public void ResetHeatSystem()
    {
        temperature = 100f; // Reset to initial temperature
        heatSystemActive = true; // Reactivate the heat system
        isGameOver = false; // Reset game over flag
        GameOverObj.SetActive(false); // Hide the Game Over screen

        // Reset egg color and Image fill color
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
        EventSystem.current.SetSelectedGameObject(GameObject.Find(GameOverObj.name + "/RestartButton"));
        GameController.instance.PauseGame(); // Pause the game
    }
}