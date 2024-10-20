using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 400.0f;
    public float lifetime = 1.0f;
    Vector2 direction;

    public void SetProjectile(Vector2 direction)
    {
        this.direction = direction;
        this.direction.Normalize();
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
}
