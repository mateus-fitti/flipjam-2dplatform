using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement; // Reference to PlayerMovement script

    public GameObject bulletPrefab;
    public Transform aimDirections; // Parent object containing aim direction children

    private Transform aimUp;
    private Transform aimDown;
    private Transform aimFront;
    private Transform aimUpFront;
    private Transform aimDownFront;

    private Transform currentBulletHole; // Reference to the current bullet hole
    public GameObject crosshairUI;
    public float aimSpeed = 5f;
    public float projectileSpeed = 10f;

    private bool canShoot = true; // Variable to track if the player can shoot
    public bool isAiming = false;
    private Vector2 aimDirection;
    private Quaternion aimRotation; // Rotation based on aim direction

    public PlayerUpgrade playerUpgrade;
    public PlayerInfo playerInfo;
    public PlayerInput playerInput;

    public GameObject bulletPrefab;

    public float projectileSpawnOffset = 0.0f;
    public float projectileMultiDistance = 0.1f;

    private bool canShoot = true; // Variable to track if the player can shoot
    private Vector2 currentMove = Vector2.zero;


    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>(); // Get reference to PlayerMovement script

        // Find aim direction children
        aimUp = aimDirections.Find("AimUp");
        aimDown = aimDirections.Find("AimDown");
        aimFront = aimDirections.Find("AimFront");
        aimUpFront = aimDirections.Find("AimUpFront");
        aimDownFront = aimDirections.Find("AimDownFront");

        // Initially disable all aim directions
        aimUp.gameObject.SetActive(false);
        aimDown.gameObject.SetActive(false);
        aimFront.gameObject.SetActive(false);
        aimUpFront.gameObject.SetActive(false);
        aimDownFront.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check if Fire1 button is pressed
        if (playerInput.actions["Fire1"].IsPressed())
        {
            if (!isAiming && canShoot)
            {
                StartAiming();
            }

            // Update aim direction based on input
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
            UpdateAimDirection(input);
        }
        else if (isAiming)
        {
            StopAimingAndFire();
        }
        //        if (playerInput.actions["Fire1"].WasReleasedThisFrame()) Fire(); //this ugly thing is back because we want to fire upon button release

    }

    private void StartAiming()
    {
        isAiming = true;
        playerMovement.canMove = false; // Prevent player movement while aiming
        StopMovement(); // Stop any ongoing movement immediately
        aimDirection = GetPlayerFacingDirection(); // Initially, the crosshair points to the direction the player is facing
        UpdateAimDirection(Vector2.zero); // Set initial aim direction
    }

    private void StopAimingAndFire()
    {
        isAiming = false;
        playerMovement.canMove = true; // Allow player movement after aiming

        // Disable all aim directions
        aimUp.gameObject.SetActive(false);
        aimDown.gameObject.SetActive(false);
        aimFront.gameObject.SetActive(false);
        aimUpFront.gameObject.SetActive(false);
        aimDownFront.gameObject.SetActive(false);

        Fire();
    }

    private void UpdateAimDirection(Vector2 input)
    {
        aimUp.gameObject.SetActive(false);
        aimDown.gameObject.SetActive(false);
        aimFront.gameObject.SetActive(false);
        aimUpFront.gameObject.SetActive(false);
        aimDownFront.gameObject.SetActive(false);

        if (input.y > 0 && input.x == 0)
        {
            aimUp.gameObject.SetActive(true);
            currentBulletHole = aimUp.Find("BulletHole");
            aimDirection = Vector2.up;
            aimRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (input.y < 0 && input.x == 0)
        {
            aimDown.gameObject.SetActive(true);
            currentBulletHole = aimDown.Find("BulletHole");
            aimDirection = Vector2.down;
            aimRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (input.y == 0 && (input.x != 0 || input == Vector2.zero))
        {
            aimFront.gameObject.SetActive(true);
            currentBulletHole = aimFront.Find("BulletHole");
            aimDirection = GetPlayerFacingDirection();
            aimRotation = Quaternion.Euler(0, 0, aimDirection == Vector2.right ? 0 : 180);
        }
        else if (input.y > 0 && input.x != 0)
        {
            aimUpFront.gameObject.SetActive(true);
            currentBulletHole = aimUpFront.Find("BulletHole");
            aimDirection = new Vector2(input.x, input.y).normalized;
            aimRotation = Quaternion.Euler(0, 0, input.x > 0 ? 45 : 135);
        }
        else if (input.y < 0 && input.x != 0)
        {
            aimDownFront.gameObject.SetActive(true);
            currentBulletHole = aimDownFront.Find("BulletHole");
            aimDirection = new Vector2(input.x, input.y).normalized;
            aimRotation = Quaternion.Euler(0, 0, input.x > 0 ? -45 : -135);
        }

        // Allow rotation while aiming
        if (input.x != 0)
        {
            playerMovement.CheckDirectionToFace(input.x > 0);
        }

        // crosshairUI.transform.position = currentBulletHole.position;
        // crosshairUI.transform.rotation = aimRotation;
    }

    private Vector2 GetPlayerFacingDirection()
    {
        // Assuming the player faces right by default
        return transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    private void Fire()
    {
        Bullet bulletScript = bulletPrefab.GetComponent<Bullet>();
        if (bulletScript == null)
        {
            Debug.LogError("Bullet prefab does not have a Bullet script attached.");
            return;
        }

        int fireCount = bulletScript.fireCount;
        float fireSpeed = bulletScript.fireSpeed;
        float fireRange = bulletScript.fireRange;
        float reloadTime = bulletScript.reloadTime;
        int fireSize = bulletScript.fireSize;

        for (int i = 0; i < fireCount; i++)
        {
            Vector3 spawnPosition = currentBulletHole.position + new Vector3(0, i * (bulletPrefab.transform.localScale.y + 0.1f), 0); // Adjust the height for multiple bullets
            GameObject go = Instantiate(bulletPrefab, spawnPosition, aimRotation);

            // Adjust the scale of the projectile based on the fireSize variable multiplied by 0.1f
            float sizeMultiplier = fireSize * 0.1f;
            go.transform.localScale = new Vector3(go.transform.localScale.x, sizeMultiplier, 1f);

            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = aimDirection.normalized * projectileSpeed;
            }

            // Ignore collision between the player and the projectile
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);

            // Re-enable collision after a short delay
            StartCoroutine(ReenableCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>()));

            Destroy(go, fireRange); // Destroy the bullet after fireRange seconds
        }

        canShoot = false; // Set canShoot to false after firing
        StartCoroutine(Reload(reloadTime)); // Start the reload coroutine with the reload time from the bullet script
    }
    
    // private void Fire(){
    //  Projectile bulletScript = bulletPrefab.GetComponent<Projectile>();
    //     if (bulletScript == null)
    //     {
    //         Debug.LogError("Prefab does not have a Projectile script attached.");
    //         return;
    //     }

    //     if (!canShoot) return;

    //     Vector3 direction;
    //     if (currentMove == Vector2.zero)
    //     {
    //         if (GetComponent<PlayerMovement>().IsFacingRight)
    //             direction = Vector3.right;
    //         else
    //             direction = Vector3.left;
    //     }
    //     else direction = new Vector3(currentMove.x, currentMove.y, 0);

    //     Vector3 dirParallel = new Vector3(-direction.y, direction.x);

    //     int multishotCount = playerUpgrade.GetMultishotCount();
    //     for (int i = 0; i < multishotCount; i++)
    //     {
    //         Vector3 spawnPosition = this.transform.position
    //             + direction * projectileSpawnOffset
    //             + dirParallel * ( projectileMultiDistance * (i - (multishotCount-1)/2));
            
    //         GameObject projectileInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
    //         Projectile projectileComp = projectileInstance.GetComponent<Projectile>();
    //         projectileComp.SetProjectile(
    //             direction,
    //             playerUpgrade.GetProjectileSpeed(),
    //             playerUpgrade.GetProjectileLifetime(),
    //             playerInfo.playerNumber
    //             );
    //     }

    //     canShoot = false;
    //     StartCoroutine(Reload(playerUpgrade.GetCooldown()));
    // }
    private IEnumerator ReenableCollision(Collider2D projectileCollider, Collider2D playerCollider)
    {
        yield return new WaitForSeconds(1f); // Wait for a short delay
        Physics2D.IgnoreCollision(projectileCollider, playerCollider, false); // Re-enable collision
    }

    IEnumerator Reload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime); // Wait for the specified reload time
        canShoot = true; // Set canShoot to true after the reload time
    }

    private void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}