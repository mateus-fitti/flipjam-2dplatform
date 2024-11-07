using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;
    private int i;

    private PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position,points[i].position) < 0.02f)
        {
            i++;
            if(i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Egg"))
        {
            collision.transform.SetParent(transform);
            if(collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject.GetComponent<PlayerMovement>();
                player.LightMovement();
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Egg"))
        {
            collision.transform.SetParent(null);
            if(collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject.GetComponent<PlayerMovement>();
                player.DefaultMovement();
            }
        }
        
    }
}