using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    public bool holded;

    // Start is called before the first frame update
    void Start()
    {
        holded = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        holded = false;
        foreach (GameObject player in players) {
            if (player.GetComponent<CharacterItemInteractions>().holdingItem)
            {
                holded = true;
            }
        }
    }
}
