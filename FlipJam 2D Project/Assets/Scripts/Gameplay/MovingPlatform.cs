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
    private Coroutine freezeCoroutine;
    private Coroutine resetFreezeCoroutine;
    private float freezeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position;
        particleSystem = GetComponent<ParticleSystem>();

        // Verifique se o sistema de partículas está configurado corretamente
        if (particleSystem != null)
        {
            Debug.Log("Particle system found on the moving platform."); // Adicionado para depuração
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
            if (player != null && !IsInHeatLayer(player))
            {
                if (freezeCoroutine != null)
                {
                    StopCoroutine(freezeCoroutine);
                }
                freezeCoroutine = StartCoroutine(FreezePlayer(player));

                // Reinicia o temporizador de reset de congelamento
                if (resetFreezeCoroutine != null)
                {
                    StopCoroutine(resetFreezeCoroutine);
                }
                resetFreezeCoroutine = StartCoroutine(ResetFreezeTime(player));
            }
        }
    }

    private bool IsInHeatLayer(PlayerMovement player)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("HeatLayer"))
            {
                freezeTime = 0f;
                return true;
            }
        }
        return false;
    }

    private IEnumerator FreezePlayer(PlayerMovement player)
    {
        player.HeavyMovement();
        player.GetComponent<SpriteRenderer>().color = Color.cyan; // Muda a cor do jogador para ciano

        freezeTime += 1f;
        yield return new WaitForSeconds(1f);

        if (freezeTime >= 5f)
        {
            SoundManager.Instance.PlaySound2D("Freeze");
            player.GetComponent<CharacterItemInteractions>().ApplyStun(Vector2.zero, 2f, 0f); // Atordoa o jogador por 2 segundos
            freezeTime = 0f; // Reseta o tempo de congelamento
        }

        freezeCoroutine = null;
    }

    private IEnumerator ResetFreezeTime(PlayerMovement player)
    {
        yield return new WaitForSeconds(1f);
        freezeTime = 0f;
        player.DefaultMovement();
        player.GetComponent<SpriteRenderer>().color = Color.white; // Restaura a cor original do jogador
    }

}