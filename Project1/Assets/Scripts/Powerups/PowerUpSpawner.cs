using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour {

	public GameObject[] powerUps;
	public float delayBetweenSpawns = 10f;
	private float timeSinceLastSpawn = 0f;
	private GameObject currentPowerUp;

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
