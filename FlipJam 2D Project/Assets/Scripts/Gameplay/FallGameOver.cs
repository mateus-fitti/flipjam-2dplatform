using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallGameOver : MonoBehaviour
{
    LevelController levelController;

    // Start is called before the first frame update
    void Start()
    {
        levelController = GameObject.Find("LevelController").GetComponent<LevelController>();    
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.transform.tag == "Player" || collision.transform.tag == "Egg")
        {
            levelController.GameOver();
        }
    }
}
