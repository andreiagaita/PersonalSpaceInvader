using UnityEngine;
using System;
using System.Collections;

public class CharacterController : MonoBehaviour
{
	public float speed = 10f;
	public float jumpForce = 5f;

	public float jumpableTimeCooldown = 0.1f;

	public GameObject graphic;
	
	public PlayerColor playerColor
	{
		get 
		{
			return GetComponent<PlayerBehaviour>().GetPlayerColor();
		}
	}

	[Flags]
	private enum CollisionFlags
	{
		Above = 1,
		Below = 2,
		Left = 4,
		Right = 8
	}
	CollisionFlags collisionFlags;
	public float collisionTestDistance = 1f;
	public float collisionTestCenterOffset = 0.4f;

	private Animator graphicAnimator;
	private Transform graphicTransform;

	private Vector3 graphicOffset;
	private Vector3 reverseGraphicOffset;

	private Vector3 graphicScale;
	private Vector3 reverseGraphicScale;

	enum VisualDirection { Left, Right }
	private VisualDirection direction = VisualDirection.Right;

	private float lastJumpableTime;

	void Start ()
	{
		graphicAnimator = graphic.GetComponent<Animator> ();
		graphicTransform = graphic.transform;

		graphicOffset = graphicTransform.localPosition;
		reverseGraphicOffset = new Vector3 (-graphicOffset.x, graphicOffset.y, graphicOffset.z);

		graphicScale = graphicTransform.localScale;
		reverseGraphicScale = new Vector3 (-graphicScale.x, graphicScale.y, graphicScale.z);
	}

	void Update ()
	{
		if ((collisionFlags & (CollisionFlags.Below | CollisionFlags.Left | CollisionFlags.Right)) != 0)
			lastJumpableTime = Time.time;

		bool canJump = jumpableTimeCooldown > (Time.time - lastJumpableTime);

		graphicAnimator.speed = Mathf.Lerp (1f, 10f, Mathf.InverseLerp (0f, speed, Mathf.Abs (rigidbody2D.velocity.x)));

		Vector2 newVelocity = rigidbody2D.velocity;

		float horizontalInput = Input.GetAxis ("Horizontal " + playerColor);

		if (canJump && Input.GetButtonDown ("Jump " + playerColor))
		{
			newVelocity.y = jumpForce;
			GetComponent<PlayerBehaviour>().RaiseJumped ("normal");
		}

		if (ShouldApplyHorizontalInput (horizontalInput))
			newVelocity.x = horizontalInput * speed;
		else
			newVelocity.x = 0f;

		rigidbody2D.velocity = newVelocity;
		
		if (direction == VisualDirection.Right && rigidbody2D.velocity.x < -0.5f)
			direction = VisualDirection.Left;
		else if (direction == VisualDirection.Left && rigidbody2D.velocity.x > 0.5f)
			direction = VisualDirection.Right;
		
		if (direction == VisualDirection.Right)
		{
			graphicTransform.localPosition = graphicOffset;
			graphicTransform.localScale = graphicScale;
		}
		else
		{
			graphicTransform.localPosition = reverseGraphicOffset;
			graphicTransform.localScale = reverseGraphicScale;
		}

		Vector3 newPosition = transform.position;
		if (transform.position.x < -0.5f && rigidbody2D.velocity.x < 0)
			newPosition = new Vector3 (newPosition.x + 32f, newPosition.y, newPosition.z);
		else if (transform.position.x > 31.5f && rigidbody2D.velocity.x > 0)
			newPosition = new Vector3 (newPosition.x - 32f, newPosition.y, newPosition.z);

		if (transform.position.y < -0.5f && rigidbody2D.velocity.y < 0)
			newPosition = new Vector3 (newPosition.x, newPosition.y + 24f, newPosition.z);
		else if (transform.position.y > 23.5f && rigidbody2D.velocity.y > 0)
			newPosition = new Vector3 (newPosition.x, newPosition.y - 24f, newPosition.z);

		transform.position = newPosition;
	}

	private bool ShouldApplyHorizontalInput (float horizontalInput)
	{
		if ((collisionFlags & (CollisionFlags.Left | CollisionFlags.Right)) == 0)
			return true;

		if ((collisionFlags & CollisionFlags.Left) != 0)
		{
			if (horizontalInput > 0)
				return true;
			else
				return false;
		}
		
		if ((collisionFlags & CollisionFlags.Right) != 0)
		{
			if (horizontalInput < 0)
				return true;
			else
				return false;
		}
			
		return false;
	}

	void FixedUpdate ()
	{
		UpdateCollisionFlags ();
	}

	private void UpdateCollisionFlags ()
	{
		collisionFlags = 0;

		if ((DirectionalCollisionTest (new Vector2 (collisionTestCenterOffset, 0f), Vector3.down)
		    || DirectionalCollisionTest (new Vector2 (-collisionTestCenterOffset, 0f), Vector3.down))
		    && rigidbody2D.velocity.y <= 0.1f)
			collisionFlags |= CollisionFlags.Below;
		
		if ((DirectionalCollisionTest (new Vector2 (collisionTestCenterOffset, 0f), Vector3.up)
		    || DirectionalCollisionTest (new Vector2 (-collisionTestCenterOffset, 0f), Vector3.up))
		    && rigidbody2D.velocity.y >= -0.1f)
			collisionFlags |= CollisionFlags.Above;
		
		if ((DirectionalCollisionTest (new Vector2 (0f, collisionTestCenterOffset), Vector3.right)
		    || DirectionalCollisionTest (new Vector2 (0f, -collisionTestCenterOffset), Vector3.right))
		    && rigidbody2D.velocity.x >= -0.1f)
			collisionFlags |= CollisionFlags.Right;
		
		if ((DirectionalCollisionTest (new Vector2 (0f, collisionTestCenterOffset), Vector3.left)
		     || DirectionalCollisionTest (new Vector2 (0f, -collisionTestCenterOffset), Vector3.left))
		    && rigidbody2D.velocity.x <= 0.1f)
			collisionFlags |= CollisionFlags.Left;
	}

	private bool DirectionalCollisionTest (Vector2 localPosition, Vector2 direction)
	{
		Vector2 position = (Vector2)transform.position + localPosition;

		bool hit = Physics2D.Raycast (position, direction, collisionTestDistance, 1 << LayerMask.NameToLayer ("Environment"));
		if (hit)
			Debug.DrawRay (position, direction * collisionTestDistance, Color.red);
		else
			Debug.DrawRay (position, direction * collisionTestDistance, Color.green);
	
		return hit;
	}
}
