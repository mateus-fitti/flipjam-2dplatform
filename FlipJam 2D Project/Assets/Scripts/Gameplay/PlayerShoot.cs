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

    public float projectileSpawnOffset = 0.0f;
    public float projectileMultiDistance = 0.1f;

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
        DisableAllAimDirections();
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
    }

    private void StartAiming()
    {
        isAiming = true;
        //playerMovement.enabled = false; // Disable player movement while aiming
        aimDirection = GetPlayerFacingDirection(); // Initially, the crosshair points to the direction the player is facing
        UpdateAimDirection(Vector2.zero); // Set initial aim direction
    }

    private void StopAimingAndFire()
    {
        isAiming = false;
        //playerMovement.enabled = true; // Enable player movement after aiming

        // Disable all aim directions
        DisableAllAimDirections();

        Fire();
    }

    private void UpdateAimDirection(Vector2 input)
    {
        DisableAllAimDirections();

        if (input.y > 0 && input.x == 0)
        {
            SetAimDirection(aimUp, Vector2.up, 90);
        }
        else if (input.y < 0 && input.x == 0)
        {
            SetAimDirection(aimDown, Vector2.down, -90);
        }
        else if (input.y == 0 && (input.x != 0 || input == Vector2.zero))
        {
            SetAimDirection(aimFront, GetPlayerFacingDirection(), GetPlayerFacingDirection() == Vector2.right ? 0 : 180);
        }
        else if (input.y > 0 && input.x != 0)
        {
            SetAimDirection(aimUpFront, new Vector2(input.x, input.y).normalized, input.x > 0 ? 45 : 135);
        }
        else if (input.y < 0 && input.x != 0)
        {
            SetAimDirection(aimDownFront, new Vector2(input.x, input.y).normalized, input.x > 0 ? -45 : -135);
        }

        // Allow rotation while aiming
        if (input.x != 0)
        {
            playerMovement.CheckDirectionToFace(input.x > 0);
        }
    }

    private void DisableAllAimDirections()
    {
        aimUp.gameObject.SetActive(false);
        aimDown.gameObject.SetActive(false);
        aimFront.gameObject.SetActive(false);
        aimUpFront.gameObject.SetActive(false);
        aimDownFront.gameObject.SetActive(false);
    }

    private void SetAimDirection(Transform aimTransform, Vector2 direction, float rotation)
    {
        aimTransform.gameObject.SetActive(true);
        currentBulletHole = aimTransform.Find("BulletHole");
        aimDirection = direction;
        aimRotation = Quaternion.Euler(0, 0, rotation);
    }

    private Vector2 GetPlayerFacingDirection()
    {
        // Assuming the player faces right by default
        return transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    private void Fire()
    {
        if (!canShoot) return;

        Projectile bulletScript = bulletPrefab.GetComponent<Projectile>();
        if (bulletScript == null)
        {
            Debug.LogError("Bullet prefab does not have a Projectile script attached!");
            return;
        }

        Vector3 direction;
        if (currentMove == Vector2.zero)
        {
            direction = GetPlayerFacingDirection() == Vector2.right ? Vector3.right : Vector3.left;
        }
        else
        {
            direction = new Vector3(currentMove.x, currentMove.y, 0);
        }

        Vector3 dirParallel = new Vector3(-direction.y, direction.x);

        int multishotCount = playerUpgrade.GetMultishotCount();
        for (int i = 0; i < multishotCount; i++)
        {
            Vector3 spawnPosition = this.transform.position
                + direction * projectileSpawnOffset
                + dirParallel * (projectileMultiDistance * (i - (multishotCount - 1) / 2));

            // Calculate the rotation angle based on the direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            GameObject projectileInstance = Instantiate(bulletPrefab, spawnPosition, rotation);
            Projectile projectileComp = projectileInstance.GetComponent<Projectile>();
            projectileComp.SetProjectile(
                direction,
                playerUpgrade.GetProjectileSpeed(),
                playerUpgrade.GetProjectileLifetime(),
                playerInfo.playerNumber
            );
        }

        canShoot = false;
        StartCoroutine(Reload(playerUpgrade.GetCooldown()));
    }

    private IEnumerator ReenableCollision(Collider2D projectileCollider, Collider2D playerCollider)
    {
        yield return new WaitForSeconds(1f); // Wait for a short delay
        Physics2D.IgnoreCollision(projectileCollider, playerCollider, false); // Re-enable collision
    }

    private IEnumerator Reload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime); // Wait for the specified reload time
        canShoot = true; // Set canShoot to true after the reload time
    }

    void OnMove(InputValue value)
    {
        currentMove = value.Get<Vector2>();
    }

    private void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        GetComponent<Animator>().Play("Idle");
    }
}