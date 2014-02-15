﻿using UnityEngine;
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
		if (playerAuraDistance < distanceLimit) {
			enemy.Die ();
			if (GameManager.instance)
				GameManager.instance.AwardPointToPlayer (this);
		}
	}
	
	public void Die () {
		if (Died != null)
			Died ();
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

	void Respawn()
	{
		if (GameManager.instance)
			transform.position = GameManager.instance.GetSpawnPositionFurtestAway ();
	}
}
