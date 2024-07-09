using UnityEngine;

public class CharacterItemInteractions : MonoBehaviour
{
    [SerializeField] private float pickupRange = 1f;
    [SerializeField] private LayerMask itemLayer;
    public bool holdingItem = false;
    GameObject item;
    PlayerMovement charMovement;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float launchForce;
    [SerializeField] private float trajactoryTimeStep = 0.05f;
    [SerializeField] private int trajactoryStepCount = 15;
    Vector2 startPosition, currentPosition;
    [SerializeField] private Vector2 velocity;

    void Awake()
    {
        charMovement = GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (!holdingItem)
            {
                PickItem();
            }
            else
            {
                ReleaseItem(Vector2.zero);
            }
        }

        if (holdingItem)
        {
            item.transform.position = transform.position;
            item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (Input.GetButtonDown("Fire1"))
            {
                //charMovement.canMove = false;
                FireProjectile();

            }

            if (Input.GetButton("Fire1"))
            {

            }

            if (Input.GetButtonUp("Fire1"))
            {

            }
        }
    }

    void PickItem()
    {
        Collider2D itemCol = Physics2D.OverlapCircle(transform.position, pickupRange, itemLayer);

        if (itemCol)
        {
            charMovement.HeavyMovement();
            item = itemCol.gameObject;
            item.GetComponent<Renderer>().enabled = false;
            holdingItem = true;
        }
    }

    // Solta o item com a velocidade do parametro, que precisa alterar o Y da velocidade do item para evitar a aceleracao continua da gravidade
    void ReleaseItem(Vector2 itemVelocity)
    {
        charMovement.canMove = true;
        holdingItem = false;
        if (item != null)
        {
            item.GetComponent<Renderer>().enabled = true;
            item.GetComponent<Rigidbody2D>().velocity = itemVelocity;
        }
        charMovement.DefaultMovement();
        item = null;
        ClearTrajectory();
    }

    // void DeleteItem()
    // {
    //     if(item != null)
    //     {
    //         Destroy(item);
    //         holdingItem = false;
    //     }
    // }

    void DrawTrajectory()
    {
        Vector3[] positions = new Vector3[trajactoryStepCount];
        for (int i = 0; i < trajactoryStepCount; i++)
        {
            float t = i * trajactoryTimeStep;
            Vector3 pos = (Vector2)item.transform.position + velocity * t + 0.5f * Physics2D.gravity * t * t;

            positions[i] = pos;
        }

        lineRenderer.positionCount = trajactoryStepCount;
        lineRenderer.SetPositions(positions);
    }

    void ClearTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
    void FireProjectile()
    {
        Rigidbody2D rg = item.GetComponent<Rigidbody2D>();
        rg.velocity = Vector2.zero;

        float horinzontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horinzontal > 0)
        {
            if (vertical > 0)
            {
                velocity = new Vector2(launchForce, 15);
            }
            else
            {
                velocity = new Vector2(launchForce, 7);
            }


        }
        else
        {
            if (vertical > 0)
            {
                velocity = new Vector2(-launchForce, 15);
            }
            else
            {
                velocity = new Vector2(-launchForce, 7);
            }
        }


        rg.AddForce(velocity, ForceMode2D.Impulse);
        //rg.velocity = velocity;

        ReleaseItem(rg.velocity);
    }
}
