using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;
    private int i;

    private PlayerMovement player;
    public bool snowstorm = false; // Variável snowstorm, falsa por padrão
    private new ParticleSystem particleSystem; // Use 'new' keyword to hide inherited member
    private Coroutine restoreCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position;
        particleSystem = GetComponentInChildren<ParticleSystem>();

        // Verifique se o sistema de partículas está configurado corretamente
        if (particleSystem != null)
        {
            var collision = particleSystem.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.sendCollisionMessages = true;
        }
        else
        {
            Debug.LogError("Particle system not found on the moving platform.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Egg"))
        {
            collision.transform.SetParent(transform);
            if (collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject.GetComponent<PlayerMovement>();
                player.LightMovement();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Egg"))
        {
            collision.transform.SetParent(null);
            if (collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject.GetComponent<PlayerMovement>();
                player.DefaultMovement();
                player.GetComponent<SpriteRenderer>().color = Color.white; // Restaura a cor original do jogador
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collision detected with: " + other.name); // Adicionado para depuração

        if (snowstorm && other.CompareTag("Player"))
        {
            Debug.Log("Snowstorm active and player detected"); // Adicionado para depuração

            player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.HeavyMovement();
                player.GetComponent<SpriteRenderer>().color = Color.blue; // Muda a cor do jogador para azul

                // Reinicia o Coroutine para restaurar o estado do jogador
                if (restoreCoroutine != null)
                {
                    StopCoroutine(restoreCoroutine);
                }
                restoreCoroutine = StartCoroutine(RestorePlayerStateAfterDelay(player));
            }
        }
    }

    private IEnumerator RestorePlayerStateAfterDelay(PlayerMovement player)
    {
        yield return new WaitForSeconds(1f); // Tempo de espera antes de restaurar o estado do jogador
        player.DefaultMovement();
        player.GetComponent<SpriteRenderer>().color = Color.white; // Restaura a cor original do jogador
    }
}