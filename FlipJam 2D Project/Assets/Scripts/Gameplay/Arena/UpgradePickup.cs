using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    private UpgradeScriptableObject currentUpgrade;

    // Define o upgrade que será aplicado
    public void SetUpgrade(UpgradeScriptableObject upgrade)
    {
        currentUpgrade = upgrade;
        GetComponent<SpriteRenderer>().sprite = upgrade.upgradeSprite;
    }

    // Detecta colisão com o jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aplica o upgrade ao jogador
            currentUpgrade.ApplyUpgrade(other.gameObject);

            // Som de coleta?

            // Destroi o upgrade após aplicação
            Destroy(gameObject);
        }
    }
}

