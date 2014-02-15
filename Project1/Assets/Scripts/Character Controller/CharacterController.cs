using UnityEngine;
using System;
using System.Collections;

public class CharacterController : MonoBehaviour
{
	public event Action<string> Jumped;

	public float speed = 10f;
	public float jumpForce = 5f;

	public GameObject graphic;

	public enum PlayerColor
	{
		Red = 0,
		Blue = 1,
		Green = 2,
		Orange = 3
	}
	public PlayerColor playerColor;

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

	private Animator graphicAnimator;
	private Transform graphicTransform;

	private Vector3 graphicOffset;
	private Vector3 reverseGraphicOffset;

	private Vector3 graphicScale;
	private Vector3 reverseGraphicScale;

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
		graphicAnimator.speed = Mathf.Lerp (1f, 10f, Mathf.InverseLerp (0f, speed, Mathf.Abs (rigidbody2D.velocity.x)));

		if (rigidbody2D.velocity.x > 0)
		{
			graphicTransform.localPosition = graphicOffset;
			graphicTransform.localScale = graphicScale;
		}
		else if (rigidbody2D.velocity.x < 0)
		{
			graphicTransform.localPosition = reverseGraphicOffset;
			graphicTransform.localScale = reverseGraphicScale;
		}

		Vector2 newVelocity = rigidbody2D.velocity;

		float horizontalInput = Input.GetAxis ("Horizontal " + playerColor);

		if (Input.GetButtonDown ("Jump " + playerColor))
		{
			newVelocity.y = jumpForce;
			if (Jumped != null)
				Jumped ("normal");
		}
		newVelocity.x = horizontalInput * speed;

		rigidbody2D.velocity = newVelocity;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay (transform.position, Vector3.up * collisionTestDistance);
		Gizmos.DrawRay (transform.position, Vector3.down * collisionTestDistance);
		Gizmos.DrawRay (transform.position, Vector3.left * collisionTestDistance);
		Gizmos.DrawRay (transform.position, Vector3.right * collisionTestDistance);

		Gizmos.color = Color.red;
		switch (collisionFlags)
		{
		case CollisionFlags.Above:
			Gizmos.DrawRay (transform.position, Vector3.up * collisionTestDistance);
			break;
		case CollisionFlags.Below:
			Gizmos.DrawRay (transform.position, Vector3.down * collisionTestDistance);
			break;
		case CollisionFlags.Left:
			Gizmos.DrawRay (transform.position, Vector3.left * collisionTestDistance);
			break;
		case CollisionFlags.Right:
			Gizmos.DrawRay (transform.position, Vector3.right * collisionTestDistance);
			break;
		}
	}

	void FixedUpdate ()
	{
		UpdateCollisionFlags ();
	}

	private void UpdateCollisionFlags ()
	{
		collisionFlags = 0;

		if (DirectionalCollisionTest (Vector3.down))
			collisionFlags |= CollisionFlags.Below;

		if (DirectionalCollisionTest (Vector3.up))
			collisionFlags |= CollisionFlags.Above;

		if (DirectionalCollisionTest (Vector3.right))
			collisionFlags |= CollisionFlags.Right;

		if (DirectionalCollisionTest (Vector3.left))
			collisionFlags |= CollisionFlags.Left;
	}

	private bool DirectionalCollisionTest (Vector2 direction)
	{
		return Physics2D.Raycast (transform.position, direction, collisionTestDistance, 1 << LayerMask.NameToLayer ("Environment"));
	}
}
