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
    private Vector2 currentMove = Vector2.zero;

    void Awake()
    {        
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.actions["Fire1"].WasReleasedThisFrame()) Fire(); //this ugly thing is back because we want to fire upon button release
    }

    void Fire()
    {
        Projectile bulletScript = bulletPrefab.GetComponent<Projectile>();
        if (bulletScript == null)
        {
            Debug.LogError("Prefab does not have a Projectile script attached.");
            return;
        }

        if (!canShoot) return;

        Vector3 direction;
        if (currentMove == Vector2.zero)
        {
            if (GetComponent<PlayerMovement>().IsFacingRight)
                direction = Vector3.right;
            else
                direction = Vector3.left;
        }
        else direction = new Vector3(currentMove.x, currentMove.y, 0);

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

    void OnMove(InputValue value)
    {
        currentMove = value.Get<Vector2>();
    }
}