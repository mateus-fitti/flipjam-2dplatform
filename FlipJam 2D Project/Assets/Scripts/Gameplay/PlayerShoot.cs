using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    private Animator animator;
    PlayerInput playerInput;

    public GameObject bullet;
    public Transform bulletHole;
    public float force = 400;
    public float reloadTime = 0.5f;

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
        // animator.SetTrigger("shoot");
        // AudioManager.instance.Play("Shoot");

        GameObject go = Instantiate(bullet, bulletHole.position, bullet.transform.rotation);
        if (GetComponent<PlayerMovement>().IsFacingRight)
            go.GetComponent<Rigidbody2D>().AddForce(Vector2.right * force);
        else
            go.GetComponent<Rigidbody2D>().AddForce(Vector2.left * force);

        Destroy(go, 1.2f);

        canShoot = false; // Set canShoot to false after firing
        StartCoroutine(Reload()); // Start the reload coroutine
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime); // Wait for 0.5 seconds
        canShoot = true; // Set canShoot to true after the reload time
    }
}