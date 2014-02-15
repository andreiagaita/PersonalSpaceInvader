using UnityEngine;
using System.Collections;

public class FastSpeed : PowerUpBase
{
	public float speedScale = 2f;
	private float originalSpeed;
	private CharacterController controller;

	public override void StartEffect (GameObject player)
	{
		controller = player.GetComponent<CharacterController>();
		originalSpeed = controller.speed;
		controller.speed *= speedScale;
	}

	public override void StopEffect (GameObject player)
	{
		controller.speed = originalSpeed;
	}
}
