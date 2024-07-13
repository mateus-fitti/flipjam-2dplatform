using UnityEngine;

public class CharacterItemInteractions : MonoBehaviour
{
    public CharacterScriptableObject characterScriptableObject;
    public CharacterType characterType;
    private HeatMeasurementSystem heatSystem;
    [SerializeField] private float pickupRange = 1f;
    [SerializeField] private LayerMask itemLayer;
    public bool holdingItem = false;
    GameObject item;
    PlayerMovement charMovement;
    Vector2 startPosition, currentPosition;
    [SerializeField] private Vector2 velocity;

    void Awake()
    {
        charMovement = GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        heatSystem = FindObjectOfType<HeatMeasurementSystem>().gameObject.GetComponent<HeatMeasurementSystem>();
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
            if(characterType == CharacterType.Aunfryn){
                heatSystem.decreaseRate = 0.1f;
            }
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
        } else if(characterType == CharacterType.Aunfryn){
            heatSystem.decreaseRate = heatSystem.defaultDecreasedRate;
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
            SoundManager.Instance.PlaySound2D("PickItem");
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
            SoundManager.Instance.PlaySound2D("ReleaseItem");
        }
        charMovement.DefaultMovement();
        item = null;
    }

    // void DeleteItem()
    // {
    //     if(item != null)
    //     {
    //         Destroy(item);
    //         holdingItem = false;
    //     }
    // }

    void FireProjectile()
    {
        Rigidbody2D rg = item.GetComponent<Rigidbody2D>();
        rg.velocity = Vector2.zero;

        float vertical = Input.GetAxis("Vertical");
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


        rg.AddForce(velocity, ForceMode2D.Impulse);
        //rg.velocity = velocity;

        ReleaseItem(rg.velocity);
    }
}
