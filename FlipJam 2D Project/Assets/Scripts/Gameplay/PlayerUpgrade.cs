using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrade : MonoBehaviour
{
    public float projectileScaleInit = 1.0f;
    public float projectileScaleStep = 1.0f;
    public float projectileLifetimeInit = 1.0f;
    public float projectileLifetimeStep = 1.0f;
    public float projectileSpeedInit = 100.0f;
    public float projectileSpeedStep = 10.0f;
    public float firerateInit = 2.0f;
    public float firerateStep = 0.5f;
    
    private int projectileScaleLevel = 0;
    private int projectileLifetimeLevel = 0;
    private int projectileSpeedLevel = 0;
    private int projectileExtraShotLevel = 0;
    private int firerateLevel = 0;

    public float GetProjectileScale()
    {
        return this.projectileScaleInit + this.projectileScaleStep * this.projectileScaleLevel;
    }

    public float GetProjectileLifetime()
    {
        return this.projectileLifetimeInit + this.projectileLifetimeStep * this.projectileLifetimeLevel;
    }

    public float GetProjectileSpeed()
    {
        return this.projectileSpeedInit + this.projectileSpeedStep * this.projectileSpeedLevel;
    }

    public int GetMultishotCount()
    {
        return 1 + this.projectileExtraShotLevel;
    }

    public float GetCooldown()
    {
        return 1 / (this.firerateInit + this.firerateStep * this.firerateLevel);
    }

    public void ProjectileScaleLevelUp()
    {
        this.projectileScaleLevel++;
    }

    public void ProjectileLifetimeLevelUp()
    {
        this.projectileLifetimeLevel++;
    }

    public void ProjectileSpeedLevelUp()
    {
        this.projectileSpeedLevel++;
    }

    public void ProjectileExtraShotLevelUp()
    {
        this.projectileExtraShotLevel++;
    }

    public void FirerateLevelUp()
    {
        this.firerateLevel++;
    }

    public void SetAllLevels(int projectileScaleLevel, int projectileLifetimeLevel, int projectileSpeedLevel, int projectileExtraShotLevel, int firerateLevel)
    {
        this.projectileScaleLevel = projectileScaleLevel;
        this.projectileLifetimeLevel = projectileLifetimeLevel;
        this.projectileSpeedLevel = projectileScaleLevel;
        this.projectileExtraShotLevel = projectileExtraShotLevel;
        this.firerateLevel = firerateLevel;
    }
}
