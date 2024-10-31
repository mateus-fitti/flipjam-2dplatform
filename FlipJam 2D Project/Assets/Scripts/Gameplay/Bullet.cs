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
  
   public enum FireSoundType { Magic, Dagger }
    public FireSoundType fireSound; // Enum for selecting the fire sound

    void Awake()
    {
        string fireSoundName = fireSound.ToString();
        SoundManager.Instance.PlaySound2D(fireSoundName, false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
