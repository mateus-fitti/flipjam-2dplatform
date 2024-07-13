/*
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Thanks so much for checking this out and I hope you find it helpful! 
	If you have any further queries, questions or feedback feel free to reach out on my twitter or leave a comment on youtube :D

	Feel free to use this in your own games, and I'd love to see anything you make!
 */

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Data")]
	public CharacterScriptableObject characterScriptableObject;

	private CharacterSpecialHabilities characterSpecialHabilities;

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
	public float jumpForce; //The actual force applied (upwards) to the player when they jump.
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
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.

	[Space(20)]

	[Header("Slide")]
	public float slideSpeed;
	public float slideAccel;

	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Header("Weights")]
	[Range(0.01f, 1f)] public float weightModifier; //Weight modifier value

	[Header("Gravity")]
	[HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
												 //Also the value the player's rigidbody2D.gravityScale is set to.

	[Space(20)]

	[Header("Run")]
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .


	#region Variables
	//Components
	public Rigidbody2D RB { get; private set; }

	private Animator animator;
	private CharacterItemInteractions itemInteractions;
	public bool isCrouching = false;
	public bool isHeavy = false;
	public bool isLight = false;
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; }

	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsSliding { get; private set; }
	public bool isGrounded { get; private set; }

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }

	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	//Can Move
	public bool canMove = true;

	private Vector2 _moveInput;
	public float LastPressedJumpTime { get; private set; }

	//Set all of these up in the inspector
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;


	[Header("Animations")]
	private string currentAnimaton;


	// Animation States
	const string PLAYER_IDLE = "Idle";
	const string PLAYER_WALK = "Walk";
	const string PLAYER_JUMP = "Jump";
	const string PLAYER_CLIMB = "Climb";
	const string PLAYER_SLIDE = "Slide";
	const string PLAYER_WALLJUMP = "WallJump";
	const string PLAYER_CROUCH = "Crouch";
	const string PLAYER_EGGIDLE = "EggIdle";
	const string PLAYER_EGGWALK = "EggWalk";
	const string PLAYER_EGGJUMP = "EggJump";
	const string PLAYER_EGGCROUCH = "EggCrouch";
	const string PLAYER_EGGCLIMB = "EggClimb";
	const string PLAYER_DEAD = "Dead";

	#endregion

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		itemInteractions = GetComponent<CharacterItemInteractions>();
		characterSpecialHabilities = GetComponent<CharacterSpecialHabilities>();
		GetScriptables();
	}

	private void Start()
	{
		OnValidate();
		SetGravityScale(gravityScale);
		IsFacingRight = true;
	}

	private void Update()
	{
		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		_moveInput.x = 0;
		_moveInput.y = 0;
		if (canMove)
		{
			_moveInput.x = Input.GetAxisRaw("Horizontal");
			_moveInput.y = Input.GetAxisRaw("Vertical");
			if (_moveInput.x != 0)
			{
				CheckDirectionToFace(_moveInput.x > 0);
			}
			if (itemInteractions)
			{
				//animator.SetBool("holdingItem", itemInteractions.holdingItem);
			}
			if (!isCrouching && Input.GetButtonDown("Jump"))
			{
				OnJumpInput();
			}
			if (Input.GetButtonDown("Jump"))
			{
				OnJumpUpInput();
			}
			if (_moveInput.y < 0 && !IsJumping && !IsSliding)
			{
				if (!itemInteractions.holdingItem)
				{
					HeavyMovement();
				}
				isCrouching = true;
			}
			else
			{
				if (!itemInteractions.holdingItem)
				{
					DefaultMovement();
				}
				isCrouching = false;
			}

			// Adjust collision ignoring based on crouching and pressing space
			int platformLayer = LayerMask.NameToLayer("Platform");
			int playerLayer = gameObject.layer;
			if (isCrouching && Input.GetButtonDown("Jump"))
			{
				Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
				StartCoroutine(ReactivateCollisionAfterDelay(0.3f));
			}
		}

		#endregion

		#region COLLISION CHECKS
		//Ground

		isGrounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);

		if (!IsJumping)
		{
			//Ground Check
			if (isGrounded && !IsJumping) //checks if set box overlaps with ground
			{
				LastOnGroundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
			}

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = coyoteTime;

			//Left Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = coyoteTime;

			//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}


		IEnumerator ReactivateCollisionAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);

			// Assuming the platform layer is named "platform"
			int platformLayer = LayerMask.NameToLayer("Platform");
			int playerLayer = gameObject.layer;

			// Reactivate collisions between the player layer and the platform layer
			Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
		}


		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			if (!IsWallJumping)
			{
				_isJumpFalling = true;
			}
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
		{
			_isJumpCut = false;

			if (!IsJumping)
				_isJumpFalling = false;
		}

		//Jump
		if (CanJump() && LastPressedJumpTime > 0)
		{
			//animator.SetBool("IsJumping", true);
			IsJumping = true;
			IsWallJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			Jump();
		}
		//WALL JUMP
		else if (CanWallJump() && LastPressedJumpTime > 0)
		{
			IsWallJumping = true;
			//animator.SetBool("IsJumping", false);
			IsJumping = false;
			_isJumpCut = false;
			_isJumpFalling = false;
			_wallJumpStartTime = Time.time;
			_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

			LimitWallJumpUses();
			characterSpecialHabilities.WallJump(_lastWallJumpDir, RB);
			Turn();
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
			IsSliding = true;
		else
		{
			IsSliding = false;
		}
		#endregion

		#region GRAVITY
		//Higher gravity if we've released the jump input or are falling
		if (IsSliding)
		{
			SetGravityScale(0);
		}
		else if (RB.velocity.y < 0 && _moveInput.y < 0)
		{
			//Much higher gravity if holding down
			SetGravityScale(gravityScale * fastFallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed));
		}
		else if (_isJumpCut)
		{
			//Higher gravity if jump button released
			SetGravityScale(gravityScale * jumpCutGravityMult);
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
		}
		else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
		{
			SetGravityScale(gravityScale * jumpHangGravityMult);
		}
		else if (RB.velocity.y < 0)
		{
			//Higher gravity if falling
			SetGravityScale(gravityScale * fallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
		}
		else
		{
			//Default gravity if standing on a platform or moving upwards
			SetGravityScale(gravityScale);
		}
		#endregion
	}

	private void LimitWallJumpUses()
	{
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;
	}

	private void FixedUpdate()
	{
		if (currentAnimaton == null) PlayEggIdleJumpAnimation();
		//Handle Run
		if (IsWallJumping)
		{
			if (!itemInteractions.holdingItem) ChangeAnimationState(PLAYER_WALLJUMP);
			else ChangeAnimationState(PLAYER_EGGCLIMB);

			Run(wallJumpRunLerp);
		}
		else
		{
			PlayEggIdleJumpAnimation();
			Run(1);
		}

		//Handle Slide
		if (IsSliding)
		{
			if (!itemInteractions.holdingItem) ChangeAnimationState(PLAYER_SLIDE);
			else ChangeAnimationState(PLAYER_EGGCLIMB);

			characterSpecialHabilities.Slide(RB);
		}
		else
		{
			PlayEggIdleJumpAnimation();
		}

		if (isCrouching)
		{
			if (!itemInteractions.holdingItem) ChangeAnimationState(PLAYER_CROUCH);
			else ChangeAnimationState(PLAYER_EGGCROUCH);
		}
		else
		{
			PlayEggIdleJumpAnimation();
		}
	}

	#region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
	public void OnJumpInput()
	{
		LastPressedJumpTime = jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}
	#endregion

	#region GENERAL METHODS
	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}
	#endregion

	//MOVEMENT METHODS
	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * runMaxSpeed;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
		{
			accelRate *= jumpHangAccelerationMult;
			targetSpeed *= jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		IsFacingRight = !IsFacingRight;
	}
	#endregion

	#region JUMP METHODS
	private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force = jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	#endregion

	#region OTHER MOVEMENT METHODS
	// private void Slide()
	// {
	// 	//Works the same as the Run but only in the y-axis
	// 	//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
	// 	float speedDif = slideSpeed - RB.velocity.y;
	// 	float movement = speedDif * slideAccel;
	// 	//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
	// 	//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
	// 	movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
	// 	RB.AddForce(movement * Vector2.up);
	// }
	#endregion


	#region CHECK METHODS
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
		{
			Turn();
		}
	}

	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}

	private bool CanWallJump()
	{
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
	{
		return IsJumping && RB.velocity.y > 0;
	}

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	public bool CanSlide()
	{
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
	#endregion

	//Unity Callback, called when the inspector updates
	private void OnValidate()
	{
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}

	#region Weight
	public void HeavyMovement()
	{
		if (!isHeavy)
		{
			runMaxSpeed *= weightModifier;
			jumpForce *= weightModifier;
			wallJumpForce *= weightModifier;
			slideSpeed /= weightModifier;
			isHeavy = true;
		}
	}
	public void LightMovement()
	{
		if (!isLight)
        {
            runMaxSpeed /= weightModifier;
            jumpForce /= weightModifier;
            wallJumpForce /= weightModifier;
            slideSpeed *= weightModifier;
            isLight = true;
        }
	}

	public void DefaultMovement()
	{
		if (isHeavy || isLight)
		{
			runMaxSpeed = characterScriptableObject.runMaxSpeed;
			jumpForce = characterScriptableObject.jumpForce;
			wallJumpForce = characterScriptableObject.wallJumpForce;
			slideSpeed = characterScriptableObject.slideSpeed;
			isHeavy = false;
		}
	}

	#endregion

	#region Animation
	//=====================================================
	// mini animation manager
	//=====================================================
	void ChangeAnimationState(string newAnimation)
	{
		if (currentAnimaton == newAnimation) return;

		foreach (var animation in characterScriptableObject.animations)
		{
			//Debug.Log(animation.animationType.ToString());
			if (animation.animationType.ToString() == newAnimation)
			{
				animator.Play(animation.animationName);
				currentAnimaton = newAnimation;
				return;
			}
		}
	}

	void PlayEggIdleJumpAnimation()
	{
		if (!itemInteractions.holdingItem)
		{
			if (isGrounded && RB.velocity.x > -0.1f && RB.velocity.x < 0.1f && !isCrouching) ChangeAnimationState(PLAYER_IDLE);
			else if (isGrounded && (RB.velocity.x < -0.1f || RB.velocity.x > 0.1f) && !isCrouching) ChangeAnimationState(PLAYER_WALK);
			else if (!isGrounded && !IsWallJumping && !IsSliding && !isCrouching) ChangeAnimationState(PLAYER_JUMP);
			else if (isCrouching) ChangeAnimationState(PLAYER_CROUCH);
			else ChangeAnimationState(PLAYER_CLIMB);
		}
		else
		{
			if (isGrounded && RB.velocity.x > -0.1f && RB.velocity.x < 0.1f && !isCrouching) ChangeAnimationState(PLAYER_EGGIDLE);
			else if (isGrounded && (RB.velocity.x < -0.1f || RB.velocity.x > 0.1f) && !isCrouching) ChangeAnimationState(PLAYER_EGGWALK);
			else if (!isGrounded && !IsWallJumping && !IsSliding && !isCrouching) ChangeAnimationState(PLAYER_EGGJUMP);
			else if (isCrouching) ChangeAnimationState(PLAYER_EGGCROUCH);
			else ChangeAnimationState(PLAYER_EGGCLIMB);
		}
	}

	public void PlayDeadAnimation()
	{
		animator.Play("Dead");
	}
	#endregion

	#region Variables Assign
	void GetScriptables()
	{
		launchForce = characterScriptableObject.launchForce;
		fallGravityMult = characterScriptableObject.fallGravityMult;
		maxFallSpeed = characterScriptableObject.maxFallSpeed;

		fastFallGravityMult = characterScriptableObject.fastFallGravityMult;
		maxFastFallSpeed = characterScriptableObject.maxFallSpeed;

		runMaxSpeed = characterScriptableObject.runMaxSpeed;
		runAcceleration = characterScriptableObject.runAcceleration;
		runDecceleration = characterScriptableObject.runDecceleration;

		accelInAir = characterScriptableObject.accelInAir;
		deccelInAir = characterScriptableObject.deccelInAir;
		doConserveMomentum = characterScriptableObject.doConserveMomentum;

		jumpHeight = characterScriptableObject.jumpHeight;
		jumpForce = characterScriptableObject.jumpForce;
		jumpTimeToApex = characterScriptableObject.jumpTimeToApex;

		jumpCutGravityMult = characterScriptableObject.jumpCutGravityMult;
		jumpHangGravityMult = characterScriptableObject.jumpHangGravityMult;
		jumpHangTimeThreshold = characterScriptableObject.jumpHangTimeThreshold;
		jumpHangAccelerationMult = characterScriptableObject.jumpHangAccelerationMult;
		jumpHangMaxSpeedMult = characterScriptableObject.jumpHangMaxSpeedMult;

		wallJumpForce = characterScriptableObject.wallJumpForce;
		wallJumpRunLerp = characterScriptableObject.wallJumpRunLerp;
		wallJumpTime = characterScriptableObject.wallJumpTime;

		slideSpeed = characterScriptableObject.slideSpeed;
		slideAccel = characterScriptableObject.slideAccel;

		coyoteTime = characterScriptableObject.coyoteTime;
		jumpInputBufferTime = characterScriptableObject.jumpInputBufferTime;

		weightModifier = characterScriptableObject.weightModifier;
	}
	#endregion
}

// created by Dawnosaur :D