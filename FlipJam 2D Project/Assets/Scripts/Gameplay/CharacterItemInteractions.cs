using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterItemInteractions : MonoBehaviour
{
    PlayerInput playerInput;
    public CharacterScriptableObject characterScriptableObject;
    public CharacterType characterType;
    private HeatMeasurementSystem heatSystem;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask itemLayer;
    public bool holdingItem = false;
    GameObject item;
    PlayerMovement charMovement;
    Vector2 startPosition, currentPosition;
    [SerializeField] private Vector2 velocity;
    private bool isThrowingItem = false; // Variable to track if the item is being thrown
    public bool isStunned = false;
    public bool isInvulnerable = false;
    private Coroutine blinkCoroutine;
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Collider2D playerCollider; // Reference to the Collider2D component
    private List<Collider2D> ignoredColliders = new List<Collider2D>(); // List to store ignored colliders

    void Awake()
    {
        charMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>(); // Initialize the Rigidbody2D component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Initialize the SpriteRenderer component
        playerCollider = GetComponent<Collider2D>(); // Initialize the Collider2D component
    }

    void Start()
    {
        if (FindObjectOfType<HeatMeasurementSystem>())
        {
            heatSystem = FindObjectOfType<HeatMeasurementSystem>().gameObject.GetComponent<HeatMeasurementSystem>();
        }
    }

    void Update()
    {
        if (playerInput.actions["Fire2"].WasPressedThisFrame())
        {
            if (!holdingItem)
            {
                PickItem();
            }
            else
            {
                Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
                if (Mathf.Abs(moveInput.x) > 0)
                {
                    FireProjectile();
                }
                else
                {
                    DropItem();
                }
            }
        }

        if (holdingItem)
        {
            if (characterType == CharacterType.Aunfryn)
            {
                heatSystem.decreaseRate = 0.1f;
            }
            item.transform.position = transform.position;
            item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else if (characterType == CharacterType.Aunfryn)
        {
            heatSystem.decreaseRate = heatSystem.defaultDecreasedRate;
        }
    }

    void PickItem()
    {
        Collider2D itemCol = Physics2D.OverlapCircle(transform.position, pickupRange, itemLayer);

        if (itemCol)
        {
            if (!itemCol.gameObject.GetComponent<ItemStatus>().holded)
            {
                charMovement.HeavyMovement();
                item = itemCol.gameObject;
                item.GetComponent<Renderer>().enabled = false;
                holdingItem = true;
                item.GetComponent<ItemStatus>().holded = true;
                SoundManager.Instance.PlaySound2D("PickItem");
            }
        }
    }

    void DropItem()
    {
        ReleaseItem(Vector2.zero);
    }

    void ReleaseItem(Vector2 itemVelocity)
    {
        charMovement.canMove = true;
        holdingItem = false;
        if (item != null)
        {
            item.GetComponent<Renderer>().enabled = true;
            item.GetComponent<Rigidbody2D>().velocity = itemVelocity;
            item.GetComponent<ItemStatus>().holded = false;
            SoundManager.Instance.PlaySound2D("ReleaseItem");
            isThrowingItem = itemVelocity.y != 0; // Set isThrowingItem based on item velocity

            // Change layer based on item velocity
            if (itemVelocity.y != 0)
            {
                item.layer = LayerMask.NameToLayer("Environment");
            }
            else
            {
                item.layer = LayerMask.NameToLayer("Pickup");
            }

            // Start monitoring the item's velocity
            StartCoroutine(MonitorItemVelocity(item.GetComponent<Rigidbody2D>()));

            // Ignore collision between the item and the player throwing it
            Collider2D itemCollider = item.GetComponent<Collider2D>();
            if (itemCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(itemCollider, playerCollider, true);
                StartCoroutine(ReenableCollision(itemCollider, playerCollider));
            }
        }
        charMovement.DefaultMovement();
        item = null;
    }

    void FireProjectile()
    {
        Rigidbody2D rg = item.GetComponent<Rigidbody2D>();
        rg.velocity = Vector2.zero;

        float vertical = playerInput.actions["Move"].ReadValue<Vector2>().y;
        if (transform.localScale.x > 0)
        {
            if (vertical > 0)
            {
                velocity = new Vector2(characterScriptableObject.launchForce, 15);
            }
            else
            {
                velocity = new Vector2(characterScriptableObject.launchForce, 7);
            }
        }
        else
        {
            if (vertical > 0)
            {
                velocity = new Vector2(-characterScriptableObject.launchForce, 15);
            }
            else
            {
                velocity = new Vector2(-characterScriptableObject.launchForce, 7);
            }
        }

        // Set the initial position of the item a bit further from the player
        item.transform.position = new Vector2(transform.position.x + (transform.localScale.x > 0 ? 1 : -1), transform.position.y);

        rg.AddForce(velocity, ForceMode2D.Impulse);
        ReleaseItem(rg.velocity);

        // Start monitoring the item's velocity
        StartCoroutine(MonitorItemVelocity(rg));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Throwable"))
        {
            Rigidbody2D itemRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Debug.Log("Item collided with " + collision.gameObject.name);

            // Stun the character if hit by a thrown item with a different character type
            Vector2 impactDirection = (transform.position - collision.transform.position).normalized;
            ApplyStun(impactDirection, 2, 5);
            isThrowingItem = false; // Reset the throwing state
        }
    }

    public void ApplyStun(Vector2 impactDirection, float stunDuration, float pushForce)
    {
        if (!isStunned)
        {
            Vector2 pushDirection = -impactDirection.normalized;
            StartCoroutine(StunCharacter(impactDirection, stunDuration, pushForce));
        }
    }

    public void ApplyInvulnerability(float invulnerabilityDuration)
    {
        if (!isInvulnerable)
        {
            StartCoroutine(Invulnerability(invulnerabilityDuration));
        }
    }

    private IEnumerator StunCharacter(Vector2 impactDirection, float stunDuration, float pushForce)
    {
        isStunned = true;
        charMovement.canMove = false;
        //rb.AddForce(impactDirection * pushForce, ForceMode2D.Force); // Apply push force
        Debug.Log("Character is stunned!");

        // Start blink effect
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(Blink(stunDuration));

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        charMovement.canMove = true;
    }

    private IEnumerator Invulnerability(float invulnerabilityDuration)
    {
        isInvulnerable = true;

        // Start blink effect
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(Blink(invulnerabilityDuration));

        yield return new WaitForSeconds(invulnerabilityDuration);

        isInvulnerable = false;
    }

    private IEnumerator Blink(float duration)
    {
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        spriteRenderer.enabled = true; // Ensure the sprite is enabled at the end
    }

    IEnumerator MonitorItemVelocity(Rigidbody2D itemRb)
    {
        while (itemRb.velocity.y != 0)
        {
            yield return null;
        }

        // Change layer back to "Pickup" when velocity.y is zero
        itemRb.gameObject.layer = LayerMask.NameToLayer("Pickup");
    }

    private IEnumerator ReenableCollision(Collider2D itemCollider, Collider2D playerCollider)
    {
        yield return new WaitForSeconds(1f); // Wait for a short delay
        Physics2D.IgnoreCollision(itemCollider, playerCollider, false); // Re-enable collision
    }
}