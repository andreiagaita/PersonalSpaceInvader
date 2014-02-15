using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	
	public static GameManager instance = null;
	
	public static int scorePlayer1 = 0;
	public static int scorePlayer2 = 0;
	public static int scorePlayer3 = 0;
	
	public List<GameObject> spawnPoints = new List<GameObject> ();
	public List<PlayerBehaviour> players = new List<PlayerBehaviour> ();
	
	public void Awake () {
		instance = this;
	}
	
	public void Start () {
		SpawnPlayers ();
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
