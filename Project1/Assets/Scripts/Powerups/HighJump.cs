using UnityEngine;
using System.Collections;

public class HighJump : MonoBehaviour 
{
	public float jumpScale = 2f;
	public float durationOfPowerUp = 10.0f;
	private float originalJumpForce;
	private float elapsedTime = 0f;
	private bool powerUpPicked = false;
	private CharacterController controller;

	void Update()
	{
		if (powerUpPicked)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > durationOfPowerUp)
			{
				controller.speed = originalJumpForce;
				DestroyImmediate(gameObject);
			}
		}
	}
	
	void OnCollisionEnter2D (Collision2D col) 
	{
		controller = col.gameObject.GetComponent<CharacterController>();
		originalJumpForce = controller.jumpForce;
		controller.jumpForce *= jumpScale;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		powerUpPicked = true;
	}
}
