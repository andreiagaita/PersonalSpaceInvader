using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	public Transform enemyAura;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 

	void Start () {
		distanceLimit = (enemyAura.renderer.bounds.size.x + transform.renderer.bounds.size.x) / 2;
	}
	
	void Update () {
		playerAuraDistance = Vector3.Distance (enemyAura.position, transform.position);
		if (playerAuraDistance < distanceLimit)
		{
			AdjustScore();
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
		DestroyImmediate(enemyAura.transform.root.gameObject);
	}

	void AdjustScore()
	{
		switch (gameObject.tag)
		{
			case "Player1" : GameManager.scorePlayer1++; break;
			case "Player2" : GameManager.scorePlayer2++; break;
			case "Player3" : GameManager.scorePlayer3++; break;
		}
		switch (enemyAura.transform.root.gameObject.tag)
		{
			case "Player1" : GameManager.scorePlayer1--; break;
			case "Player2" : GameManager.scorePlayer2--; break;
			case "Player3" : GameManager.scorePlayer3--; break;
		}
		Debug.Log ("P1: " + GameManager.scorePlayer1 +
		           " P2: " + GameManager.scorePlayer2 +
		           " P3: " + GameManager.scorePlayer3);
	}
}
