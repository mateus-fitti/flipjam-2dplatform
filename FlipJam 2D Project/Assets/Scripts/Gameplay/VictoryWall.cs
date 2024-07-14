using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VictoryWall : MonoBehaviour
{
    LevelController levelController;
    [SerializeField] float boxSize = 5;
    bool eggArrive;
    bool playerArrive;

    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        eggArrive = false;
        playerArrive = false;
    }

    void Update()
    {
        if (eggArrive && playerArrive)
        {
            levelController.Victory();
        }
        eggArrive = false;
        playerArrive = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(boxSize, boxSize), 0);
        
        foreach (Collider2D col in colliders)
        {
            if (col.transform.tag == "Egg")
            {
                eggArrive = true;
            }
            if (col.transform.tag == "Player")
            {
                playerArrive = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(boxSize,boxSize,boxSize));
    }
}
