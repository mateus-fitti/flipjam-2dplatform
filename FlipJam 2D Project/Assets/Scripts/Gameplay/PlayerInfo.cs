using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int playerNumber = 1;
    public int maxHealth = 3; // Vida inicial: 3 corações
    public int currentHealth;
    public int limitHearts = 5; // Máximo de vida: 5 corações

    public GameObject indicator;

    private HealthUI healthUI;
    private CharacterItemInteractions itemInteractions;

    void Awake()
    {
        currentHealth = maxHealth;
        healthUI = FindObjectOfType<HealthUI>();
        itemInteractions = GetComponent<CharacterItemInteractions>();
        UpdateHealthUI();
        UpdateIndicator(playerNumber);
    }

    public void ApplyDamage(int damage, Vector2 impactDirection)
    {
        if (itemInteractions != null && !itemInteractions.isInvulnerable)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");
            UpdateHealthUI();
            SoundManager.Instance.PlaySound2D("Hit", false);
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                StartCoroutine(HandleDeath());
            }
            else
            {
                itemInteractions.ApplyStun(impactDirection, 1f, 1f); // Stun for 1 second with a push force of 5
                itemInteractions.ApplyInvulnerability(3f); // Invulnerable for 2 seconds
            }
        }
    }

    private IEnumerator HandleDeath()
    {
        // Play death animation
        GetComponent<PlayerMovement>().PlayDeadAnimation();
        Animator animator = GetComponent<Animator>();
        // Wait for the death animation to finish
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0)
        .normalizedTime >= 1f &&
         !animator.IsInTransition(0));

        // Wait for an additional 1 second
        yield return new WaitForSeconds(1f);

        // Notify the LevelController to handle game over
        LevelController levelController = FindObjectOfType<LevelController>();
        if (levelController != null)
        {
            levelController.GameOver();
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        if (maxHealth > limitHearts)
        {
            maxHealth = limitHearts;
        }
        //currentHealth = maxHealth; // Optionally, set current health to new max health
        Heal(amount);
        Debug.Log($"{gameObject.name} increased max health. Current max health: {maxHealth}");
        UpdateHealthUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log($"{gameObject.name} healed. Current health: {currentHealth}");
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealthUI(playerNumber, currentHealth, maxHealth);
        }
    }

    private void UpdateIndicator(int playerNumber)
    {
        this.playerNumber = playerNumber;
        if (indicator != null)
        {
            SpriteRenderer indicatorSprite = indicator.GetComponent<SpriteRenderer>();
            TextMesh indicatorText = indicator.GetComponentInChildren<TextMesh>();
            if (indicatorText) indicatorText.text = playerNumber.ToString();

            if (indicatorSprite)
            {

                if (playerNumber == 1)
                {
                    indicatorSprite.color = Color.red;
                }
                else if (playerNumber == 2)
                {
                    indicatorSprite.color = Color.cyan; // Dark blue
                }

            }
        }
    }

    public void SetPlayerNumber(int playerNumber)
    {
        UpdateIndicator(playerNumber);
        UpdateHealthUI();
    }
}