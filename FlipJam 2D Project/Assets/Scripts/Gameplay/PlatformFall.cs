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

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        timeCounter = timeToFall;
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
            }
            if (timeCounter < -respawnTime)
            {
                ResetPosition();
            }
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

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.tag == "Player" && col.transform.position.y > originalPosition.y)
        {
            col.transform.SetParent(transform);
            falling = true;
        }
    }

    void OnCollisionExit2D (Collision2D col)
    {
        col.transform.SetParent(null);
    }
}
