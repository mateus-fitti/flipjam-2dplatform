using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    private float fallDelay = 3f;
    private float destroyDelay = 7f;
    [SerializeField] private Rigidbody2D rb;
    private CharacterItemInteractions characterInteractions;

    void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            characterInteractions = collision.gameObject.GetComponent<CharacterItemInteractions>();
            if(characterInteractions.holdingItem)
                StartCoroutine(Fall()); 
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }
    

}
