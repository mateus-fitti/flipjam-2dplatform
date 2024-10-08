using UnityEngine;

public class BulletUpgradeManager : MonoBehaviour
{
    public GameObject bulletObject;
    private Bullet bullet;

    // Variáveis para armazenar os valores iniciais do Bullet
    private float initialFireSpeed;
    private int initialFireSize;
    private int initialFireCount;
    private float initialFireRange;
    private float initialReloadTime;

    void Awake()
    {
        if (bulletObject == null)
        {
            Debug.LogError("Bullet reference is not set in BulletUpgradeManager.");
        }
        else
        {
            bullet = bulletObject.GetComponent<Bullet>();
            if (bullet == null)
            {
                Debug.LogError("Bullet component not found on bulletObject.");
            }
        }
    }

    void Start()
    {
        // Armazenar os valores iniciais do Bullet
        if (bullet != null)
        {
            initialFireSpeed = bullet.fireSpeed;
            initialFireSize = bullet.fireSize;
            initialFireCount = bullet.fireCount;
            initialFireRange = bullet.fireRange;
            initialReloadTime = bullet.reloadTime;
        }

        // Resetar os valores do Bullet assim que o script for instanciado
        ResetUpgrades();
    }

    public void UpgradeFireSpeed(float amount)
    {
        if (bullet != null)
        {
            bullet.fireSpeed = bullet.fireSpeed + amount;
            Debug.Log("Fire speed upgraded to: " + bullet.fireSpeed);
        }
    }

    public void UpgradeFireSize(int amount)
    {
        if (bullet != null)
        {
            bullet.fireSize = bullet.fireSize + amount;
            Debug.Log("Fire size upgraded to: " + bullet.fireSize);
        }
    }

    public void UpgradeFireCount(int amount)
    {
        if (bullet != null)
        {
            bullet.fireCount += amount;
            Debug.Log("Fire count upgraded to: " + bullet.fireCount);
        }
    }

    public void UpgradeFireRange(float amount)
    {
        if (bullet != null)
        {
            bullet.fireRange = bullet.fireRange + amount;
            Debug.Log("Fire range upgraded to: " + bullet.fireRange);
        }
    }

    public void UpgradeReloadTime(float amount)
    {
        if (bullet != null)
        {
            bullet.reloadTime = bullet.reloadTime + amount;
            if (bullet.reloadTime < 0)
            {
                bullet.reloadTime = 0;
            }
            Debug.Log("Reload time upgraded to: " + bullet.reloadTime);
        }
    }

    // Método para resetar os upgrades (para facilitar o teste)
    public void ResetUpgrades()
    {
        if (bullet != null)
        {
            bullet.fireSpeed = initialFireSpeed;
            bullet.fireSize = initialFireSize;
            bullet.fireCount = initialFireCount;
            bullet.fireRange = initialFireRange;
            bullet.reloadTime = initialReloadTime;
            Debug.Log("Upgrades resetados para os valores iniciais.");
        }
    }
}