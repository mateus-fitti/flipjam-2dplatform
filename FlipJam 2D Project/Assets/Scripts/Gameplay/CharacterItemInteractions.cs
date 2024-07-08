using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterItemInteractions : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask itemLayer;
    public bool holdingItem = false;
    GameObject item;
    CharacterMovement charMovement;

    // Start is called before the first frame update
    void Start()
    {
        charMovement = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!holdingItem)
            {
                PickItem();
            } else {
                ReleaseItem();
            }
        }

        if (holdingItem)
        {
            item.transform.position = transform.position;
        }
    }

    void PickItem()
    {
        Collider2D itemCol = Physics2D.OverlapCircle(transform.position, pickupRange, itemLayer);

        if (itemCol)
        {
            charMovement.HeavyMovement();
            item = itemCol.gameObject;
            holdingItem = true;
        }
    }

    void ReleaseItem()
    {
        charMovement.DefaultMovement();
        item = null;
        holdingItem = false;
    }
}
