using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	public Transform enemyAura;
	public Transform spawnLocation;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 
	private GameObject enemy;

	void Start () {
		distanceLimit = (enemyAura.renderer.bounds.size.x + transform.renderer.bounds.size.x) / 2;
		transform.position = spawnLocation.position;
		enemy = enemyAura.transform.root.gameObject;
	}
	
	void Update () {
		playerAuraDistance = Vector3.Distance (enemyAura.position, transform.position);
		if (playerAuraDistance < distanceLimit)
		{
			AdjustScore();
			RespawnEnemy();
			Debug.Log(GameManager.playersAlive);
			if (GameManager.playersAlive > 2)
			{
				//KillEnemy();
				//re-assign player colors
			}
			else
			{
				//Display screen with winners
			}
		}
	}

	void KillEnemy()
	{
		GameManager.playersAlive--;
		DestroyImmediate(gameObject);
	}

	void AdjustScore()
	{
		switch (gameObject.tag)
		{
			case "Player1" : GameManager.scorePlayer1--; break;
			case "Player2" : GameManager.scorePlayer2--; break;
			case "Player3" : GameManager.scorePlayer3--; break;
		}
		switch (enemy.tag)
		{
			case "Player1" : GameManager.scorePlayer1++; break;
			case "Player2" : GameManager.scorePlayer2++; break;
			case "Player3" : GameManager.scorePlayer3++; break;
		}
		Debug.Log ("P1: " + GameManager.scorePlayer1 +
		           " P2: " + GameManager.scorePlayer2 +
		           " P3: " + GameManager.scorePlayer3);
	}

	void RespawnEnemy()
	{
		enemy.transform.position = enemy.GetComponent<PlayerBehaviour>().spawnLocation.position;
		transform.position = spawnLocation.position;
	}
}
