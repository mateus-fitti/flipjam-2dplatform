using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100.0f;
    public float lifetime = 1.0f;
    public int owner = 0;
    Vector2 direction;

    public void SetProjectile(Vector2 direction, float speed, float lifetime, int owner)
    {
        this.direction = direction;
        this.direction.Normalize();
        this.speed = speed;
        this.lifetime = lifetime;
        this.owner = owner;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        Vector2 displace = direction * speed * Time.fixedDeltaTime;
        transform.Translate(new Vector3(displace.x, displace.y, 0));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerInfo info = collider.GetComponent<PlayerInfo>();
            if (info.playerNumber != owner)
            {
                Debug.Log("bullet collided with " + collider.gameObject.name + ", todo damage here");
            }
        }
    }
}
