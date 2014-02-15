using UnityEngine;
using System.Collections;

public class HighJump : PowerUpBase
{
	public float jumpScale = 2f;
	private float originalJumpForce;
	private CharacterController controller;

	public override void StartEffect (GameObject player)
	{
		controller = player.GetComponent<CharacterController>();
		originalJumpForce = controller.jumpForce;
		controller.jumpForce *= jumpScale;
	}

	public override void StopEffect (GameObject player)
	{
		controller.jumpForce = originalJumpForce;
	}
}
