using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodableWall : MonoBehaviour
{
    private Explodable explodable;
    [SerializeField] private LayerMask pickupLayer;
    // Start is called before the first frame update
    void Start()
    {
        explodable = GetComponent<Explodable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((pickupLayer.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            explodable.explode();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
