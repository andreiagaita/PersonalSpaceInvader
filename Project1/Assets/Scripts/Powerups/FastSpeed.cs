using UnityEngine;
using System.Collections;

public class FastSpeed : MonoBehaviour 
{
	public float speedScale = 2f;
	public float durationOfPowerUp = 10.0f;
	private float originalSpeed;
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
				controller.speed = originalSpeed;
				DestroyImmediate(gameObject);
			}
		}
	}
	
	void OnCollisionEnter2D (Collision2D col) 
	{
		controller = col.gameObject.GetComponent<CharacterController>();
		originalSpeed = controller.speed;
		controller.speed *= speedScale;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		powerUpPicked = true;
	}
}
