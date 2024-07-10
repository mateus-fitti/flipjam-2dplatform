using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpecialHabilities 
{
    public CharacterScriptableObject characterScriptableObject;

    private void EnhancedLaunchForce(){
        characterScriptableObject.launchForce = characterScriptableObject.launchForce * 1.5;
    }
    private void EnhancedLaunchForce(){
        characterScriptableObject.jumpForce = characterScriptableObject.jumpForce * 1.5;
    }
    private void WallJump(int dir, Rigidbody2D RB)
	{
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(characterScriptableObject.wallJumpForce.x, characterScriptableObject.wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
			force.y -= RB.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss
		RB.AddForce(force, ForceMode2D.Impulse);
	}
}
