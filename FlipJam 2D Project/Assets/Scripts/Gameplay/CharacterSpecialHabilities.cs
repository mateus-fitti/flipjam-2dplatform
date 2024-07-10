using UnityEngine;

public class CharacterSpecialHabilities : MonoBehaviour
{
    public CharacterScriptableObject characterScriptableObject;

    void Start()
    {

    }

    void Update()
    {

    }

    public void EnhancedLaunchForce(){
        // characterScriptableObject.launchForce = characterScriptableObject.launchForce * 1.5;
    }
    public void EnhancedJumpForce(){
        // characterScriptableObject.jumpForce = characterScriptableObject.jumpForce * 1.5;
    }
    public void WallJump(int dir, Rigidbody2D RB)
	{
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

    public void Slide(Rigidbody2D RB)
	{
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		float speedDif = characterScriptableObject.slideSpeed - RB.velocity.y;
		float movement = speedDif * characterScriptableObject.slideAccel;
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));


		RB.AddForce(movement * Vector2.up);
	}
}
