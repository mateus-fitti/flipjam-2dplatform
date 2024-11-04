using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Sprite heartSprite; // Sprite do coração cheio
    public Sprite emptyHeartSprite; // Sprite do coração vazio
    public Transform player1HealthContainer; // Container para os corações do jogador 1
    public Transform player2HealthContainer; // Container para os corações do jogador 2

    private List<Image> player1Hearts = new List<Image>();
    private List<Image> player2Hearts = new List<Image>();

    public void UpdateHealthUI(int playerNumber, int currentHealth, int maxHealth)
    {
        List<Image> hearts;
        Transform container;

        if (playerNumber == 1)
        {
            hearts = player1Hearts;
            container = player1HealthContainer;
        }
        else
        {
            hearts = player2Hearts;
            container = player2HealthContainer;
        }

        // Remove corações extras
        while (hearts.Count > maxHealth)
        {
            Destroy(hearts[hearts.Count - 1].gameObject);
            hearts.RemoveAt(hearts.Count - 1);
        }

        // Adiciona corações se necessário
        while (hearts.Count < maxHealth)
        {
            Image heart = new GameObject("Heart").AddComponent<Image>();
            heart.transform.SetParent(container);
            heart.transform.localScale = new Vector3(1, 1, 1); // Define a escala do coração
            hearts.Add(heart);
        }

        // Atualiza a visibilidade dos corações
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentHealth ? heartSprite : emptyHeartSprite;
            hearts[i].gameObject.SetActive(true);
        }
    }
}