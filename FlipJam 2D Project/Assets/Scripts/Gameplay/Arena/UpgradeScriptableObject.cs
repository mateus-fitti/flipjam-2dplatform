using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades/New Upgrade", order = 1)]
public class UpgradeScriptableObject : ScriptableObject
{
    public string upgradeName;  // Nome do upgrade, caso precise
    public Sprite upgradeSprite; // O sprite que será mostrado
    public AnimationClip animationClip; // Animação (se tiver)
    public UpgradeType type; // Enum para definir qual tipo de upgrade é

     // Parâmetros que podem ser usados dependendo do tipo de upgrade
    public int healthIncreaseAmount;
    public float speedIncreaseAmount;
    public float shieldDuration;

    // Função abstrata que será implementada pelos upgrades específicos
    public virtual void ApplyUpgrade(GameObject player)
    {
        switch (type)
        {
            case UpgradeType.ProjectileScale:
                player.GetComponent<PlayerUpgrade>().ProjectileScaleLevelUp();
                break;
            case UpgradeType.ProjectileRange:
                player.GetComponent<PlayerUpgrade>().ProjectileLifetimeLevelUp();
                break;
            case UpgradeType.ProjectileSpeed:
                player.GetComponent<PlayerUpgrade>().ProjectileSpeedLevelUp();
                break;
            case UpgradeType.ProjectileExtra:
                player.GetComponent<PlayerUpgrade>().ProjectileExtraShotLevelUp();
                break;
            case UpgradeType.ProjectileFirerate:
                player.GetComponent<PlayerUpgrade>().FirerateLevelUp();
                break;
            case UpgradeType.Health:
                player.GetComponent<PlayerInfo>().Heal(healthIncreaseAmount);
                break;
            case UpgradeType.HealthMax:
                player.GetComponent<PlayerInfo>().IncreaseMaxHealth(1);
                break;
            default:
                Debug.LogWarning("Tipo de upgrade não implementado.");
                break;
        }

        // Cada Scriptable Object pode sobrescrever essa função
        Debug.Log($"Aplicando upgrade: {upgradeName}");
    }
}

public enum UpgradeType
{
    ProjectileScale,
    ProjectileRange,
    ProjectileSpeed,
    ProjectileExtra,
    ProjectileFirerate,
    Health,
    HealthMax,
}
