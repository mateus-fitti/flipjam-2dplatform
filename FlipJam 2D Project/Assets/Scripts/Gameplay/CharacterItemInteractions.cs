using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterItemInteractions : MonoBehaviour
{
    [SerializeField] private float pickupRange = 1f;
    [SerializeField] private LayerMask itemLayer;
    public bool holdingItem = false;
    GameObject item;
    CharacterMovement charMovement;

    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private Transform spawPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float launchForce = 1.5f;
    [SerializeField] private float trajactoryTimeStep = 0.05f;
    [SerializeField] private int trajactoryStepCount = 15;
    Vector2 velocity, startPosition, currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        charMovement = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
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

            if(Input.GetButtonDown("Fire1"))
            {
                charMovement.canMove = false;
                startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                
            }

            if(Input.GetButton("Fire1"))
            {
                currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                velocity = (startPosition - currentPosition) * launchForce;

                DrawTrajectory();
            }

            if(Input.GetButtonUp("Fire1"))
            {
                charMovement.canMove = true;
                FireProjectile();
                DeleteItem();
                ClearTrajectory();
            }
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

    void DeleteItem()
    {
        if(item != null)
        {
            Destroy(item);
            holdingItem = false;
        }
    }

    void DrawTrajectory()
    {
        Vector3[] positions = new Vector3[trajactoryStepCount];
        for (int i = 0; i < trajactoryStepCount;i++)
        {
            float t = i * trajactoryTimeStep;
            Vector3 pos = (Vector2)spawPoint.position + velocity * t + 0.5f * Physics2D.gravity * t * t;

            positions[i] = pos;
        }

        lineRenderer.positionCount = trajactoryStepCount;
        lineRenderer.SetPositions(positions);
    }

    void ClearTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
    void FireProjectile()
    {
        Transform pr = Instantiate(projectilePrefab, spawPoint.position, Quaternion.identity);
        pr.GetComponent<Rigidbody2D>().velocity = velocity;
    }
}
