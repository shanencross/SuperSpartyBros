using UnityEngine;
using UnityEngine.SceneManagement; // to load scenes and/or get current scene
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

	// player controls
	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 3f;

	public float jumpForce = 600f;
	public float bounceForce = 600f;

	// player health
	public int playerHealth = 1;

	// LayerMask to determine what is considered ground for the player
	public LayerMask whatIsGround;

	// Transform just below feet for checking if player is grounded
	public Transform groundCheck;

	// Circle collider for physics collision with ground
	public GameObject groundColliderObject;

	// Box collider for physics and trigger collisions with environment
	public GameObject environmentColliderObject;

	public GameObject groundCollider2object;

	// round y position to nearest 10th unit when _isGrounded flag is true
	public bool roundYpositionWhenGrounded = false;

	// player can move?
	// we want this public so other scripts can access it but we don't want to show in editor as it might confuse designer
	[HideInInspector]
	public bool playerCanMove = true;

	// SFXs
	public AudioClip coinSFX;
	public AudioClip deathSFX;
	public AudioClip fallSFX;
	public AudioClip jumpSFX;
	public AudioClip bounceSFX;
	public AudioClip victorySFX;

	// private variables below

	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
	SpriteRenderer _spriteRenderer;
	CircleCollider2D _groundCollider;
	BoxCollider2D _environmentCollider;
	BoxCollider2D _groundCollider2;

	// hold player motion in this timestep
	float _vx;
	float _vy;

	// player tracking
	bool _facingRight = true;
	bool _isGrounded = false;
	bool _isRunning = false;
	bool _canDoubleJump = true;

	// store the layer the player is on (setup in Awake)
	int _playerLayer;

	// number of layer that Platforms are on (setup in Awake)
	int _platformLayer;
	
	void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (_spriteRenderer==null)
			Debug.LogError ("SpriterRenderer component missing from this gameobject");

		if (bounceSFX == null) {
			Debug.LogWarning("BounceSFX not set. Setting to JumpSFX by default.");
			bounceSFX = jumpSFX;
		}
			
		if (groundColliderObject == null) {
			groundColliderObject = _transform.Find("GroundCollider").gameObject;

			if (groundColliderObject == null) {
				Debug.LogError("GroundCollider child object not attached to player");
			}
		}

		_groundCollider = groundColliderObject.GetComponent<CircleCollider2D>();
		if (_groundCollider == null) {
			Debug.LogError("CircleCollider2D component missing from GroundCollider child object");
		}

		if (environmentColliderObject == null) {
			environmentColliderObject = _transform.Find("EnvironmentCollider").gameObject;

			if (environmentColliderObject == null) {
				Debug.LogError("EnvironmentCollider child object not attached to player");
			}
		}

		_environmentCollider = environmentColliderObject.GetComponent<BoxCollider2D>();
		if (_environmentCollider == null) {
			Debug.LogError("BoxCollider2D component missing from EnvironmentCollider child object");
		}

		if (groundCollider2object == null) {
			groundCollider2object = _transform.Find("GroundCollider2").gameObject;

			if (groundCollider2object == null) {
				Debug.LogError("GroundCollider2 child object not attached to player");
			}
		}

		_groundCollider2 = groundCollider2object.GetComponent<BoxCollider2D>();
		if (_groundCollider2 == null) {
			Debug.LogError("CircleCollider2D component missing from GroundCollider2 child object");
		}

		// determine the player's specified layer
		_playerLayer = this.gameObject.layer;

		// determine the platform's specified layer
		_platformLayer = LayerMask.NameToLayer("Platform");

		// if (GameManager.gm.GetComponent<ItemList>().items);
	}

	// this is where most of the player controller magic happens each game event loop
	void Update() {
		// exit update if player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
			return;

		// determine horizontal velocity change based on the horizontal input
		_vx = CrossPlatformInputManager.GetAxisRaw ("Horizontal");

		// Determine if running based on the horizontal movement
		if (_vx != 0) 
		{
			_isRunning = true;
		} else {
			_isRunning = false;
		}

		// set the running animation state
		_animator.SetBool("Running", _isRunning);

		// get the current vertical velocity from the rigidbody component
		_vy = _rigidbody.velocity.y;


		// get coordinates for top of circle collider
		Vector3 topOfGroundCollider_local = new Vector3(_groundCollider.offset.x, _groundCollider.offset.y + _groundCollider.radius, 0);
		//Vector3 topOfGroundCollider_local = new Vector3(0f, -0.135f + 0.067f, 0f);
		Vector3 topOfGroundCollider = _transform.position + topOfGroundCollider_local;

		// Check to see if character is grounded by linecasting from the top of the circle collider
		// down to the groundCheck position and see if connected with gameobjects on the
		// whatIsGround layer
		_isGrounded = Physics2D.Linecast(topOfGroundCollider, groundCheck.position, whatIsGround);  

		// Set the grounded animation states
		_animator.SetBool("Grounded", _isGrounded);

		if (_isGrounded) {
			if (roundYpositionWhenGrounded) {
				// eliminate character bouncing up and down while walking
				// due to janky physics
				roundYPosition();
			}
				
			//_groundCollider2.enabled = true;
			//_environmentCollider.isTrigger = false;
			_canDoubleJump = true;
		} 
		else {
			//_groundCollider2.enabled = false;
			//_environmentCollider.isTrigger = true;
		}

		if(CrossPlatformInputManager.GetButtonDown("Jump")) // If grounded AND jump button pressed, then allow the player to jump
		{
			if 	(_isGrounded) {
				Jump ();
			}
			// double jump if in mid-air and if double jump hasn't been used up yet,
			// but only if double jump ability is in item inventory,
			// or if no item inventory exists
			else if (_canDoubleJump && (!ItemInventory.iv || (ItemInventory.iv && ItemInventory.iv.doubleJump))) {
				Jump();
				_canDoubleJump = false;
			}
		}
	
		// If the player stops jumping mid jump and player is not yet falling
		// then set the vertical velocity to 0 (he will start to fall from gravity)
		if (CrossPlatformInputManager.GetButtonUp ("Jump") && _vy > 0f) {
			_vy = 0f;
		} 

		// Change the actual velocity on the rigidbody
		_rigidbody.velocity = new Vector2(_vx * moveSpeed, _vy);

		// if moving up then don't collide with platform layer
		// this allows the player to jump up through things on the platform layer
		// NOTE: requires the platforms to be on a layer named "Platform"
		Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, (_vy > 0.0f)); 
	}

	// Checking to see if the sprite should be flipped
	// this is done in LateUpdate since the Animator may override the localScale
	// this code will flip the player even if the animator is controlling scale
	void LateUpdate()
	{
		// get the current scale
		Vector3 localScale = _transform.localScale;

		if (_vx > 0) // moving right so face right
		{
			_facingRight = true;
		} else if (_vx < 0) { // moving left so face left
			_facingRight = false;
		}

		// check to see if scale x is right for the player
		// if not, multiple by -1 which is an easy way to flip a sprite
		if (((_facingRight) && (localScale.x<0)) || ((!_facingRight) && (localScale.x>0))) {
			localScale.x *= -1;
		}

		// update the scale
		_transform.localScale = localScale;
	}

	void VerticalBoost(float force, AudioClip SFX) {
		// reset current vertical motion to 0 prior to jump
		_vy = 0f;
		// add a force in the up direction
		_rigidbody.AddForce(new Vector2 (0, force));
		// play the jump sound
		PlaySound(SFX);
	}

	void Jump() {
		VerticalBoost (jumpForce, jumpSFX);
	}

	// if the player collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag=="MovingPlatform") {
			this.transform.parent = other.transform;
		}
	}

	// if the player exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag=="MovingPlatform") {
			this.transform.parent = null;
		}
	}

	// do what needs to be done to freeze the player
 	void FreezeMotion() {
		playerCanMove = false;
		_rigidbody.bodyType = RigidbodyType2D.Static;
		//_rigidbody.isKinematic = true;
	}

	// do what needs to be done to unfreeze the player
	void UnFreezeMotion() {
		playerCanMove = true;
		_rigidbody.bodyType = RigidbodyType2D.Dynamic;
		//_rigidbody.isKinematic = false;
	}

	// play sound through the audiosource on the gameobject
	void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}

	// public function to apply damage to the player
	public void ApplyDamage(int damage) {
		if (playerCanMove) {
			playerHealth -= damage;

			if (playerHealth <= 0) { // player is now dead, so start dying
				PlaySound(deathSFX);
				StartCoroutine (KillPlayer ());
			}
		}
	}

	// public function to kill the player when they have a fall death
	public void FallDeath() {
		if (playerCanMove) {
			playerHealth = 0;
			PlaySound(fallSFX);
			StartCoroutine (KillPlayer ());
		}
	}

	// coroutine to kill the player
	IEnumerator KillPlayer()
	{
		if (playerCanMove)
		{
			// freeze the player
			FreezeMotion();

			// play the death animation
			_animator.SetTrigger("Death");
			
			// After waiting tell the GameManager to reset the game
			yield return new WaitForSeconds(2.0f);

			if (GameManager.gm) // if the gameManager is available, tell it to reset the game
				GameManager.gm.ResetGame();
			else // otherwise, just reload the current level
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	public void CollectCoin(int amount) {
		PlaySound(coinSFX);

		if (GameManager.gm) // add the points through the game manager, if it is available
			GameManager.gm.AddPoints(amount);
	}

	// public function on victory over the level
	public void Victory() {
		PlaySound(victorySFX);
		FreezeMotion ();
		//place sprite in foreground in front of everything else for victory animation
		_spriteRenderer.sortingLayerName = "Foreground";
		_spriteRenderer.sortingOrder = 20;
		//play victory animation
		_animator.SetTrigger("Victory");

		if (GameManager.gm) // do the game manager level compete stuff, if it is available
			GameManager.gm.LevelCompete();
	}

	public void Bounce() {
		VerticalBoost (bounceForce, bounceSFX);
	}
		
	// public function to respawn the player at the appropriate location
	public void Respawn(Vector3 spawnloc) {
		UnFreezeMotion();
		playerHealth = 1;
		_transform.parent = null;
		_transform.position = spawnloc;
		_animator.SetTrigger("Respawn");
	}

	public void roundYPosition() {
		// round to nearest 0.1 multiple
		float yPositionRounded = Mathf.Round(_transform.position.y * 10) / 10;

		//round to nearest 0.25 multiple
		//float yPositionRounded = Mathf.Round(_transform.position.y * 4) / 4;

		_transform.position = new Vector3(_transform.position.x, yPositionRounded, transform.position.z);
	}
}
