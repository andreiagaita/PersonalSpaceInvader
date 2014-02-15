using UnityEngine;
using System.Collections;
using System;

public class PlayerBehaviour : MonoBehaviour {

	public event Action Died;
	public Transform enemyAura;
	public Transform spawnLocation;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 
	private GameObject enemy;

	void Awake () {
		GameManager.instance.players.Add (this);
	}
	
	void Start () {
		distanceLimit = (enemyAura.renderer.bounds.size.x) / 2;
		enemy = enemyAura.transform.root.gameObject;
	}
	
	void Update () {
		if (!enemyAura || !spawnLocation)
			return;

		playerAuraDistance = Vector3.Distance (enemyAura.position, transform.position);
		if (playerAuraDistance < distanceLimit)
		{
			if (Died != null)
				Died ();
			AdjustScore();
			RespawnEnemy();
		}
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
