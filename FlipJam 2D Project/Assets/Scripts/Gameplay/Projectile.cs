using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100.0f;
    public float lifetime = 1.0f;
    public int owner = 0;
    public int damage = 1; // Damage dealt by the projectile
    Vector2 direction;

    public enum FireSoundType { Magic, Dagger }
    public FireSoundType fireSound; // Enum for selecting the fire sound

    public void SetProjectile(Vector2 direction, float speed, float lifetime, int owner)
    {
        this.direction = direction;
        this.direction.Normalize();
        this.speed = speed;
        this.lifetime = lifetime;
        this.owner = owner;
    }

    void Awake()
    {
        string fireSoundName = fireSound.ToString();
        SoundManager.Instance.PlaySound2D(fireSoundName, false);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        // Use the rotation of the projectile to determine the direction
        Vector2 displace = transform.right * speed * Time.fixedDeltaTime;
        transform.Translate(displace, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerInfo info = collider.GetComponent<PlayerInfo>();
            if (info != null && info.playerNumber != owner)
            {
                Debug.Log("Projectile hit player!");
                Vector2 impactDirection = (collider.transform.position - transform.position).normalized;
                info.ApplyDamage(damage, impactDirection);
                Destroy(gameObject); // Destroy the projectile after hitting the player
            }
        }
        else if (collider.gameObject.layer == LayerMask.NameToLayer("Environment") || collider.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            Destroy(gameObject); // Destroy the projectile after hitting the environment or pickup layer
        }
    }
}