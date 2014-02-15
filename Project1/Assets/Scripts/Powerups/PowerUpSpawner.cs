using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {

	public GameObject[] powerUps;
	private float delayBetweenSpawns;
	private float timeSinceLastSpawn = 0f;
	private GameObject currentPowerUp;

	void Start()
	{
		delayBetweenSpawns = Random.Range(5, 15);
	}

	void Update () {
		timeSinceLastSpawn += Time.deltaTime;
		if (!currentPowerUp && (timeSinceLastSpawn > delayBetweenSpawns))
		{
			var index = Random.Range (0, powerUps.Length);
			currentPowerUp = Instantiate(powerUps[index], transform.position, Quaternion.identity) as GameObject;
			timeSinceLastSpawn = 0f;
		}
	}
}
