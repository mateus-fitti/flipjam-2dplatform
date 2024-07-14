using UnityEngine;

public class CharacterSpecialHabilities : MonoBehaviour
{
    public CharacterType characterType;
    public CharacterScriptableObject characterScriptableObject;
    public LayerMask collisionLayers;

    public GameObject castingLight;
    public GameObject range;

    // Declaração da variável startTime
    private float startTime;
    Rigidbody2D rig2D;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 2f;
    public bool isDashing = false;
    private float dashTimeLeft;

    [Header("Teleport")]
    public GameObject energySpherePrefab;
    public float maxTeleportDistance = 10f;
    public float initialSphereSpeed = 1f;
    public float sphereAcceleration = 0.5f;
    public float maxSphereAcceleration = 1.5f;
    public float tremorIntensity = 0.1f;
    public float tremorThreshold = 2f;
    public Transform handPosition;
    public PlayerMovement playerMovementScript;

    private GameObject energySphere;
    private bool isCasting = false;
    private float currentSphereSpeed;
    private Vector2 castDirection;
    private Vector2 initialPosition;

    void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        rig2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (characterType == CharacterType.Aunfryn)
        {
            if (Input.GetButtonDown("Dash"))
            {
                StartCasting();
                castingLight.SetActive(true);
                range.SetActive(true);
            }

            if (isCasting)
            {
                UpdateCasting();
            }

            if (Input.GetButtonUp("Dash"))
            {
                castingLight.SetActive(false);
                range.SetActive(false);
                Teleport();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                castingLight.SetActive(false);
                range.SetActive(false);
                CancelCasting();
            }
        }
        else if (characterType == CharacterType.Ngoro)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
            if (Input.GetButtonDown("Dash"))
            {
                if (!isDashing)
                {
                    Dash();
                }
            }
        }

    }

    #region Ngoro
    void Dash()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        isDashing = true;
        dashTimeLeft = dashTime;
        rig2D.velocity = new Vector2(horizontal * dashSpeed, rig2D.velocity.y);

    }
    #endregion

    #region Aunfryn
    void StartCasting()
    {
        if(!isCasting) { SoundManager.Instance.PlaySound2D("Cast"); }
        isCasting = true;
        initialPosition = handPosition != null ? handPosition.position : transform.position;

        energySphere = Instantiate(energySpherePrefab, initialPosition, Quaternion.identity);
        currentSphereSpeed = initialSphereSpeed;
        if (playerMovementScript != null)
        {
            playerMovementScript.canMove = false;
        }
    }

    void UpdateCasting()
    {
        //SoundManager.Instance.PlaySound2D("Cast");
        castDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        // Acelerar a esfera até o máximo permitido
        if (castDirection.magnitude > 0 && currentSphereSpeed < maxSphereAcceleration)
        {
            currentSphereSpeed += sphereAcceleration * Time.deltaTime;
        }

        Vector2 newPosition2D = (Vector2)energySphere.transform.position + castDirection * currentSphereSpeed * Time.deltaTime;

        // Detectar colisões
        Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition2D, energySphere.GetComponent<CircleCollider2D>().radius, collisionLayers);
        bool collidedWithEnvironment = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Environment") || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                // Ajustar a posição para evitar atravessar camadas "Environment" e "Platform"
                Vector2 direction = (newPosition2D - (Vector2)energySphere.transform.position).normalized;
                newPosition2D = (Vector2)energySphere.transform.position + direction * (collider.Distance(energySphere.GetComponent<Collider2D>()).distance - 0.1f);
                collidedWithEnvironment = true;
                break;
            }
        }

        // Aplicar nova posição
        energySphere.transform.position = (Vector3)newPosition2D;

        // Se colidiu com o ambiente, reduzir a velocidade gradualmente ao invés de parar totalmente
        if (collidedWithEnvironment)
        {
            currentSphereSpeed *= 0.5f; // Reduzir a velocidade ao colidir com o ambiente
        }

        // Iniciar contagem de tempo no lançamento da esfera
        if (castDirection.magnitude > 0 && startTime == 0f)
        {
            startTime = Time.time;
        }

        // Aplicar tremor nos primeiros 1 segundo de lançamento da esfera
        if (castDirection.magnitude > 0 && Time.time - startTime < 1f)
        {
            ApplyTremor();
        }

        // Reiniciar startTime quando a direção é zero
        if (castDirection.magnitude <= 0)
        {
            startTime = 0f;
        }

        // Limitar a distância da esfera
        if (Vector2.Distance(initialPosition, (Vector2)energySphere.transform.position) > maxTeleportDistance)
        {
            Vector2 direction = ((Vector2)energySphere.transform.position - initialPosition).normalized;
            energySphere.transform.position = (Vector2)initialPosition + direction * maxTeleportDistance;
        }
    }

    void ApplyTremor()
    {
        float tremorX = Random.Range(-tremorIntensity, tremorIntensity);
        float tremorY = Random.Range(-tremorIntensity, tremorIntensity);
        energySphere.transform.position += new Vector3(tremorX, tremorY, 0);
    }




    void Teleport()
    {
        if (isCasting)
        {
            SoundManager.Instance.PlaySound2D("Teleport");
            isCasting = false;
            transform.position = energySphere.transform.position;
            Destroy(energySphere);
            if (playerMovementScript != null)
            {
                playerMovementScript.canMove = true;
            }
        }
    }

    void CancelCasting()
    {
        if (isCasting)
        {
            isCasting = false;
            Destroy(energySphere);
            if (playerMovementScript != null)
            {
                playerMovementScript.canMove = true;
            }
        }
    }

    #endregion

    public void EnhancedLaunchForce()
    {
        // characterScriptableObject.launchForce = characterScriptableObject.launchForce * 1.5;
    }
    public void EnhancedJumpForce()
    {
        // characterScriptableObject.jumpForce = characterScriptableObject.jumpForce * 1.5;
    }
    public void WallJump(int dir, Rigidbody2D RB)
    {
        Vector2 force = new Vector2(characterScriptableObject.wallJumpForce.x, characterScriptableObject.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        RB.AddForce(force, ForceMode2D.Impulse);
    }

    public void Slide(Rigidbody2D RB)
    {
        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = characterScriptableObject.slideSpeed - RB.velocity.y;
        float movement = speedDif * characterScriptableObject.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));


        RB.AddForce(movement * Vector2.up);
    }
}
