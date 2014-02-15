using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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
	
	public List<GameObject> spawnPoints = new List<GameObject> ();
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
		if (PlayerCreated != null) {
			foreach (var player in players) {
				PlayerCreated (player.tag);
			}
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
}
