﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum PlayerColor
{
	Red = 0,
	Blue = 1,
	Green = 2,
	Orange = 3
}

public class GameManager : MonoBehaviour {

	public event Action<PlayerBehaviour> PlayerCreated;
	static GameManager gameManager = null;
	public static GameManager instance {
		get { return gameManager; }
		private set {
			if (gameManager != null)
			{
				Destroy (value.gameObject);
				value.enabled = false;
			}
			else
			{
				gameManager = value;
				DontDestroyOnLoad (gameManager.gameObject);
			}
		}
	}
	public GameObject soundManagerPrefab;
	public GUIText scoreTextPrefab;
	
	public Dictionary<PlayerColor, int> scoreDict = new Dictionary<PlayerColor, int> ();
	public Dictionary<PlayerColor, GUIText> scoreTexts = new Dictionary<PlayerColor, GUIText> ();
	
	public Color[] playerColors;
	public float assignNewTargetsDelay = 20.0f;
	public GameObject pulsatingAura;
	private GameObject[] pulsatingAuras;
	private bool aurasPulsating = false;
	private float timeSinceLastTargetReassign = 0f;
	private float pulsatingAuraNotificationLength = 3.0f;

	[HideInInspector]
	public List<GameObject> spawnPoints = new List<GameObject> ();
	[HideInInspector]
	public List<PlayerBehaviour> players = new List<PlayerBehaviour> ();
	
	public void Awake () {
		var old = gameManager;
		instance = this;
		if (old == null)
			Instantiate (soundManagerPrefab);
	}
	
	public void OnLevelWasLoaded (int level) {
		SpawnPlayers ();
		AssignTargets ();
		
		for (int i=0; i<players.Count; i++) {
			var player = players[i];
			GUIText text = Instantiate (scoreTextPrefab) as GUIText;
			text.pixelOffset = new Vector2 (40, -10-30*i);
			text.color = player.GetActualPlayerColor ();
			text.text = "0";
			
			scoreDict[player.playerColor] = 0;
			scoreTexts[player.playerColor] = text;
		}
	}

	public void Update()
	{
		timeSinceLastTargetReassign += Time.deltaTime;
		if (!aurasPulsating && (timeSinceLastTargetReassign > assignNewTargetsDelay - pulsatingAuraNotificationLength))
		{
			NotifyIncomingTargetReassignments();
		}

		if (timeSinceLastTargetReassign > assignNewTargetsDelay)
		{
			AssignTargets();
			DestroyPulsatingAuraCircles();
			timeSinceLastTargetReassign = 0f;
		}
	}

	public void AddPlayer (PlayerBehaviour player)
	{
		players.Add (player);
		if (PlayerCreated != null)
			PlayerCreated (player);
	}

	public void SpawnPlayers () {
		List<GameObject> randomSpawnPoints = new List<GameObject> (spawnPoints);
		randomSpawnPoints.Shuffle ();
		foreach (var player in players) {
			player.transform.position = randomSpawnPoints[0].transform.position;
			randomSpawnPoints.RemoveAt (0);
		}
	}

	public void AssignTargets()
	{
		players.Shuffle();
		for (var i = 0; i < players.Count; ++i)
		{
			players[i].enemy = players[(i + 1) % players.Count];
			players[i].enemy.aura.GetComponent<SpriteRenderer>().color = players[i].GetActualPlayerColor();
		}
	}
	
	public Vector3 GetSpawnPositionFurtestAway () {
		int spawnIndex = -1;
		float maxDist = 0;
		for (int i=0; i<spawnPoints.Count; i++) {
			float minDist = Mathf.Infinity;
			for (int j=0; j<players.Count; j++) {
				float dist = Vector2.Distance (players[j].transform.position, spawnPoints[i].transform.position);
				minDist = Mathf.Min (minDist, dist);
			}
			
			if (minDist > maxDist)
			{
				maxDist = minDist;
				spawnIndex = i;
			}
		}
		return spawnPoints[spawnIndex].transform.position;
	}
	
	public void AwardPointToPlayer (PlayerBehaviour player) {
		scoreDict[player.playerColor] += 1;
		scoreTexts[player.playerColor].text = "" + scoreDict[player.playerColor];
	}

	private void NotifyIncomingTargetReassignments()
	{
		pulsatingAuras = new GameObject[players.Count];
		for (var i = 0; i < players.Count; ++i)
		{
			pulsatingAuras[i] = Instantiate(pulsatingAura, players[i].transform.position, Quaternion.identity) as GameObject;
			pulsatingAuras[i].transform.parent = players[i].transform;
		}
		aurasPulsating = true;
	}

	private void DestroyPulsatingAuraCircles()
	{
		for (var i = 0; i < players.Count; ++i)
			DestroyImmediate(pulsatingAuras[i]);
		aurasPulsating = false;
	}
}
