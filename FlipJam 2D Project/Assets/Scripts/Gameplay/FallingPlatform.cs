using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FallingPlatform : MonoBehaviour
{

    private float fallDelay = 1f;
    private float destroyDelay = 7f;
    // private float respawnTime = 5f;
    // private Vector3 spawnPosition;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask playerLayer;

    void Start()
    {
        //spawnPosition = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //StartCoroutine(Respawn());
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }
    
    // private IEnumerator Respawn()
    // {
    //     yield return new WaitForSeconds(respawnTime);
    //     rb.bodyType = RigidbodyType2D.Kinematic;
    //     Instantiate(gameObject,spawnPosition, transform.rotation);
    // }

}
