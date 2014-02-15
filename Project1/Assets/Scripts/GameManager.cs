using UnityEngine;
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

	public event Action<string> PlayerCreated;
	static GameManager gameManager = null;
	public static GameManager instance {
		get { return gameManager; }
		private set {
			if (gameManager != null)
				Destroy (gameManager.gameObject);
			gameManager = value;
			DontDestroyOnLoad (gameManager.gameObject);
		}
	}
	public GameObject soundManagerPrefab;
	
	public static int scorePlayer1 = 0;
	public static int scorePlayer2 = 0;
	public static int scorePlayer3 = 0;
	public Color[] playerColors;
	public float assignNewTargetsDelay = 20.0f;
	private float timeSinceLastTargetReassign = 0f;

	[HideInInspector]
	public List<GameObject> spawnPoints = new List<GameObject> ();
	[HideInInspector]
	public List<PlayerBehaviour> players = new List<PlayerBehaviour> ();
	
	public void Awake () {
		if (instance == null)
		{
			instance = this;
			Instantiate (soundManagerPrefab);
		}
	}
	
	public void Start () {
		SpawnPlayers ();
		AssignTargets ();
		if (PlayerCreated != null) {
			foreach (var player in players) {
				PlayerCreated (player.tag);
			}
		}
	}

	public void Update()
	{
		timeSinceLastTargetReassign += Time.deltaTime;
		if (timeSinceLastTargetReassign > assignNewTargetsDelay)
		{
			AssignTargets();
			timeSinceLastTargetReassign = 0f;
		}
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
}
