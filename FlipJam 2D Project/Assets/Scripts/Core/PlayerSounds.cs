using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayJumpSound()
{
    SoundManager.Instance.PlaySound2D("Jump");
}
    public void PlayStepSound()
{
    //SoundManager.Instance.PlaySound2D("Step");
}

public void PlayWallJumpSound()
{
    SoundManager.Instance.PlaySound2D("WallJump");
}

public void PlayGameOverSound()
{
    SoundManager.Instance.PlaySound2D("GameOver");
}

}
