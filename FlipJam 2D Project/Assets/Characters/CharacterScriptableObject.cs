using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Characters", order = 1)]
public class CharacterScriptableObject : ScriptableObject
{
	[Header("Launch Force")]
	public float launchForce;

    [Header("Gravity")]
	public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
	public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
	[Space(5)]
	public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
									  //Seen in games such as Celeste, lets the player fall extra fast if they wish.
	public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach.
	public float deafaultMaxSpeed; //Default target speed we want the player to reach.
	public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[Space(5)]
	[Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float deafaultJumpHeight; //Default Height of the player's jump
	public float jumpForce; //The actual force applied (upwards) to the player when they jump.
	public float deafaultJumpForce; //Default force applied (upwards) to the player when they jump.
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.

	[Header("Both Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	public float jumpHangAccelerationMult;
	public float jumpHangMaxSpeedMult;

	[Header("Wall Jump")]
	public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
	public Vector2 deafaultWallJumpForce; // The deafault force applied to the player when wall jumping.
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.

	[Space(20)]

	[Header("Slide")]
	public float slideSpeed;
	public float deafaultSlideSpeed;
	public float slideAccel;

	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Header("Weights")]
	[Range(0.01f, 1f)] public float weightModifier; //Weight modifier value

	[System.Serializable]
    public class AnimationClipData
    {
        public AnimationType animationType;
        public AnimationClip animationClip;
        public string animationName => animationClip != null ? animationClip.name : string.Empty;
    }

    public enum AnimationType
    {
        Idle,
        Walk,
        Jump,
        Climb,
        Slide,
        WallJump,
        Crouch,
        EggIdle,
        EggWalk,
        EggJump,
		EggCrouch,
        EggClimb,
		Dead
    }

	[Header("Animations")]
    public List<AnimationClipData> animations = new List<AnimationClipData>();

    private void OnEnable()
    {
        // Ensure the animations list matches the enum size
        if (animations.Count != System.Enum.GetNames(typeof(AnimationType)).Length)
        {
            animations.Clear();
            foreach (AnimationType type in System.Enum.GetValues(typeof(AnimationType)))
            {
                animations.Add(new AnimationClipData { animationType = type });
            }
        }
    }

	[CustomEditor(typeof(CharacterScriptableObject))]
	public class CharacterScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Cast the target object to the correct type
        CharacterScriptableObject scriptableObject = (CharacterScriptableObject)target;

        // Draw the default inspector options
        DrawDefaultInspector();

        // Custom GUI for the animations list
        EditorGUILayout.LabelField("Animations");
        if (scriptableObject.animations != null)
        {
            for (int i = 0; i < scriptableObject.animations.Count; i++)
            {
                // Use the animation type as the label for each list item
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(scriptableObject.animations[i].animationType.ToString());
                scriptableObject.animations[i].animationClip = (AnimationClip)EditorGUILayout.ObjectField(scriptableObject.animations[i].animationClip, typeof(AnimationClip), false);
                EditorGUILayout.EndHorizontal();
            }
        }

        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
}