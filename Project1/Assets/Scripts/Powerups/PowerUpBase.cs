using UnityEngine;
using System.Collections;

public abstract class PowerUpBase : MonoBehaviour 
{
	public float durationOfPowerUp = 10.0f;
	private float elapsedTime = 0f;
	private bool powerUpPicked = false;
	private GameObject player;
	
	void Update()
	{
		if (powerUpPicked)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > durationOfPowerUp)
			{
				StopEffect(player);
				Destroy(gameObject);
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		player = col.gameObject;
		var playerBehaviour = player.GetComponent<PlayerBehaviour>();
		if (playerBehaviour.activePowerUp != null) {
			playerBehaviour.activePowerUp.StopEffect(player);
			Destroy(playerBehaviour.activePowerUp);
		}
		playerBehaviour.activePowerUp = this;

		StartEffect(player);

		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		powerUpPicked = true;
	}

	public abstract void StartEffect(GameObject player);

	public abstract void StopEffect(GameObject player);
}
