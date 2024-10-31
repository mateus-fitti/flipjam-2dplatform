using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public CharacterType characterType = CharacterType.Aunfryn;

    public float fireSpeed = 400;
    public int fireSize = 1;
    public int fireCount = 1;
    public float fireRange = 1.2f;

    public float reloadTime = 0.5f;
  
    public string fireSound; // Reference to the audio clip for the fire sound

    void Awake()
    {
        
        if (fireSound != null)
        {
           SoundManager.Instance.PlaySound2D(fireSound, false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
