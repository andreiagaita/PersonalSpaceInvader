using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class PlayerController : XInputController
{
	public float force;
	public float speed;

	public void Update()
	{
		base.Update();

		if (!controllerReady)
			return;

		float horizontal = state.ThumbSticks.Left.X;
		float vertical = state.ThumbSticks.Left.Y;		

		rigidbody2D.AddForce(new Vector2(horizontal * force, vertical * force));
		rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -speed, speed), Mathf.Clamp(rigidbody2D.velocity.y, -speed, speed));
	}
}