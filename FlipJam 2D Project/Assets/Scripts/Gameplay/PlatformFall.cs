using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    [SerializeField] float timeToFall = 2f;
    float timeCounter;
    [SerializeField] float respawnTime = 5f;
    [SerializeField] float fallSpeed = 5f;
    bool falling = false;
    Vector3 originalPosition;
    Collider2D platformCollider;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        timeCounter = timeToFall;
        platformCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            timeCounter -= Time.deltaTime;

            if (timeCounter <= 0f)
            {
                FallMovement(Time.deltaTime);
                platformCollider.isTrigger = true; // Set isTrigger to true when falling
            }
            if (timeCounter < -respawnTime)
            {
                ResetPosition();
            }
        }
        else
        {
            platformCollider.isTrigger = false; // Set isTrigger to false when not falling
        }
    }

    void FallMovement(float time)
    {
        transform.position = transform.position + (Vector3.down * fallSpeed * time);
    }

    void ResetPosition()
    {
        timeCounter = timeToFall;
        falling = false;
        transform.position = originalPosition;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player" && col.transform.position.y > originalPosition.y)
        {
            //col.transform.SetParent(transform);
            falling = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        //col.transform.SetParent(null);
    }
}