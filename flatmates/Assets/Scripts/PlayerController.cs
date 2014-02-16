using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class PlayerController : MonoBehaviour
{
	public float force;
	public float speed;

	public enum ControllerType { None, Xbox, Keyboard }
	public ControllerType controller;

    public int controllerID = 0;
    public int playerID = 0;

	public void Update()
	{
        //base.Update();

        //if (controller == ControllerType.Xbox && !controllerReady)
        //    return;

		float horizontal = 0;
		float vertical = 0;
		
		switch (controller)
		{
			case ControllerType.Keyboard:
                horizontal = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
                vertical = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
				break;

			case ControllerType.Xbox:
                //horizontal = state.ThumbSticks.Left.X;
                //vertical = state.ThumbSticks.Left.Y;
                horizontal = Input.GetAxis("P" + controllerID + "Horizontal");
                vertical = Input.GetAxis("P" + controllerID + "Vertical");
				break;
		}

		rigidbody2D.AddForce(new Vector2(horizontal * force, vertical * force));
		rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -speed, speed), Mathf.Clamp(rigidbody2D.velocity.y, -speed, speed));
	}
}