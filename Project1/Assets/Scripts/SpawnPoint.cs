using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GameManager.instance.spawnPoints.Add (gameObject);
	}
}
