using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class PlayerController : XInputController
{
	public float force;
	public float speed;

	public enum ControllerType { None, Xbox, Keyboard }
	public ControllerType controller;

	public void Update()
	{
		base.Update();

		if (controller == ControllerType.Xbox && !controllerReady)
			return;

		float horizontal = 0;
		float vertical = 0;
		
		switch (controller)
		{
			case ControllerType.Keyboard:
				horizontal = Input.GetAxis("P" + playerNumber + "Horizontal");
				vertical = Input.GetAxis("P" + playerNumber + "Vertical");
				break;

			case ControllerType.Xbox:
				horizontal = state.ThumbSticks.Left.X;
				vertical = state.ThumbSticks.Left.Y;
				break;
		}

		rigidbody2D.AddForce(new Vector2(horizontal * force, vertical * force));
		rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -speed, speed), Mathf.Clamp(rigidbody2D.velocity.y, -speed, speed));
	}
}