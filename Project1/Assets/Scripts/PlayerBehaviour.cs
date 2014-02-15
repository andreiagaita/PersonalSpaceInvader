using UnityEngine;
using System.Collections;
using System;

public class PlayerBehaviour : MonoBehaviour {

	public event Action Died;
	public PlayerBehaviour enemy;
	public GameObject aura;
	public GameObject arrow;
	public Transform spawnLocation;
	public PlayerColor playerColor;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 

	void Awake () {
		if (GameManager.instance)
			GameManager.instance.players.Add (this);
	}
	
	void Start () {
		distanceLimit = (aura.renderer.bounds.size.x) / 2;
		arrow.GetComponent<SpriteRenderer>().color = GetActualPlayerColor();
	}
	
	void Update () {
		if (!enemy) return;

		playerAuraDistance = Vector3.Distance (enemy.transform.position, transform.position);
		if (playerAuraDistance < distanceLimit)
			enemy.Die ();
	}
	
	public void Die () {
		if (Died != null)
			Died ();
		AdjustScore();
		Respawn();
	}

	public PlayerColor GetPlayerColor()
	{
		return playerColor;
	}

	public Color GetActualPlayerColor()
	{
		if (GameManager.instance)
			return GameManager.instance.playerColors[(int)playerColor];
		return Color.green;
	}

	void AdjustScore()
	{
		/*switch (gameObject.tag)
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
		           " P3: " + GameManager.scorePlayer3);*/
	}

	void Respawn()
	{
		if (GameManager.instance)
			transform.position = GameManager.instance.GetSpawnPositionFurtestAway ();
	}
}
