using UnityEngine;
using System;
using System.Collections;

public class CharacterController : MonoBehaviour
{
	public float speed = 10f;
	public float jumpForce = 5f;

	public float collisionTestDistance = 1f;

	[Flags]
	private enum CollisionFlags
	{
		Above = 1,
		Below = 2,
		Left = 4,
		Right = 8
	}
	CollisionFlags collisionFlags;

	void Update ()
	{
		Vector2 newVelocity = rigidbody2D.velocity;

		if (Input.GetButtonDown ("Jump"))
			newVelocity.y = jumpForce;
		newVelocity.x = Input.GetAxis ("Horizontal") * speed;

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
