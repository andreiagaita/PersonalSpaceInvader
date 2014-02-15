using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PlayerColor
{
	Red = 0,
	Blue = 1,
	Green = 2,
	Orange = 3
}

public class GameManager : MonoBehaviour {
	
	public static GameManager instance = null;
	
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
		instance = this;
	}
	
	public void Start () {
		SpawnPlayers ();
		AssignTargets ();
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
			players[i].aura.GetComponent<SpriteRenderer>().color = players[i].enemy.GetActualPlayerColor();
		}
	}
}
