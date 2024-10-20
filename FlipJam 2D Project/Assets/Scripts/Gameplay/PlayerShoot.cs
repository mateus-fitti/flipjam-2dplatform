using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    private Animator animator;
    PlayerInput playerInput;

    public GameObject bulletPrefab;

    public float projectileSpawnOffset = 0.0f;
    public float projectileSize = 1.0f;
    public float projectileMultiDistance = 0.1f;
    public int projectileMultishot = 1;
    public float projectileLifetime = 1.0f;
    public float rateOfFire = 0.5f;

    private bool canShoot = true; // Variable to track if the player can shoot

    void Awake()
    {        
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnFire1()
    {
        Projectile bulletScript = bulletPrefab.GetComponent<Projectile>();
        if (bulletScript == null)
        {
            Debug.LogError("Prefab does not have a Projectile script attached.");
            return;
        }

        if (!canShoot) return;

        Vector3 direction = Vector3.right; //todo actually get direction
        Vector3 dirParallel = new Vector3(-direction.y, direction.x);

        for (int i = 0; i < projectileMultishot; i++)
        {
            Vector3 spawnPosition = this.transform.position
                + direction * projectileSpawnOffset
                + dirParallel * ( projectileMultiDistance * (i - (projectileMultishot-1)/2));
            
            GameObject projectileInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Projectile projectileComp = projectileInstance.GetComponent<Projectile>();
            projectileComp.SetProjectile(direction);
        }

        canShoot = false;
        StartCoroutine(Reload(rateOfFire));
    }

    IEnumerator Reload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime); // Wait for the specified reload time
        canShoot = true; // Set canShoot to true after the reload time
    }
}