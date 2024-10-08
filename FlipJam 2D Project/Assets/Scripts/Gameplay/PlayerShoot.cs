using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    private Animator animator;
    PlayerInput playerInput;

    public GameObject bulletPrefab;
    public Transform bulletHole;

    private bool canShoot = true; // Variable to track if the player can shoot

    void Awake()
    {        
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.actions["Fire1"].WasPressedThisFrame() && canShoot)
        {
            Fire();
        }
    }

    void Fire()
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
        Vector3 spawnPosition = bulletHole.position + new Vector3(0, i * (bulletPrefab.transform.localScale.y + 0.1f), 0); // Adjust the height for multiple bullets
        GameObject go = Instantiate(bulletPrefab, spawnPosition, bulletPrefab.transform.rotation);
        
        // Ajustar a escala do projétil com base na variável fireSize multiplicada por 0.1f
        float sizeMultiplier = fireSize * 0.1f;
        go.transform.localScale = new Vector3(go.transform.localScale.x, sizeMultiplier, 1f);

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (GetComponent<PlayerMovement>().IsFacingRight)
                rb.AddForce(Vector2.right * fireSpeed);
            else
                rb.AddForce(Vector2.left * fireSpeed);
        }

        Destroy(go, fireRange); // Destroy the bullet after fireRange seconds
    }

    canShoot = false; // Set canShoot to false after firing
    StartCoroutine(Reload(reloadTime)); // Start the reload coroutine with the reload time from the bullet script
}
    IEnumerator Reload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime); // Wait for the specified reload time
        canShoot = true; // Set canShoot to true after the reload time
    }
}