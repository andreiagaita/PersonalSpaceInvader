﻿using UnityEngine;
using System;
using System.Collections;

public class CharacterController : MonoBehaviour
{
	public event Action<string> Jumped;

	public float speed = 10f;
	public float jumpForce = 5f;

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
		bool canJump = false;

		if ((collisionFlags & (CollisionFlags.Below | CollisionFlags.Left | CollisionFlags.Right)) != 0)
			canJump = true;

		graphicAnimator.speed = Mathf.Lerp (1f, 10f, Mathf.InverseLerp (0f, speed, Mathf.Abs (rigidbody2D.velocity.x)));

		Vector2 newVelocity = rigidbody2D.velocity;

		float horizontalInput = Input.GetAxis ("Horizontal " + playerColor);

		if (canJump && Input.GetButtonDown ("Jump " + playerColor))
		{
			newVelocity.y = jumpForce;
			if (Jumped != null)
				Jumped ("normal");
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
		    && rigidbody2D.velocity.y <= 0)
			collisionFlags |= CollisionFlags.Below;
		
		if ((DirectionalCollisionTest (new Vector2 (collisionTestCenterOffset, 0f), Vector3.up)
		    || DirectionalCollisionTest (new Vector2 (-collisionTestCenterOffset, 0f), Vector3.up))
		    && rigidbody2D.velocity.y >= 0)
			collisionFlags |= CollisionFlags.Above;
		
		if ((DirectionalCollisionTest (new Vector2 (0f, collisionTestCenterOffset), Vector3.right)
		    || DirectionalCollisionTest (new Vector2 (0f, -collisionTestCenterOffset), Vector3.right))
		    && rigidbody2D.velocity.x >= 0)
			collisionFlags |= CollisionFlags.Right;
		
		if ((DirectionalCollisionTest (new Vector2 (0f, collisionTestCenterOffset), Vector3.left)
		     || DirectionalCollisionTest (new Vector2 (0f, -collisionTestCenterOffset), Vector3.left))
		    && rigidbody2D.velocity.x <= 0)
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
