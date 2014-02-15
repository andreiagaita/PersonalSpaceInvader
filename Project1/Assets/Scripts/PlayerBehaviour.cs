using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	public Transform enemyAura;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 

	void Start () {
		distanceLimit = (enemyAura.renderer.bounds.size.x + transform.renderer.bounds.size.x) / 2;
	}
	
	void Update () {
		playerAuraDistance = Vector3.Distance (enemyAura.position, transform.position);
		if (playerAuraDistance < distanceLimit)
		{
			Debug.Log(GameManager.playersAlive);
			if (GameManager.playersAlive > 2)
			{
				KillEnemy();
			}
			else
			{
				//Display screen with winners
			}
		}
	}

	void KillEnemy()
	{
		GameManager.playersAlive--;
		DestroyImmediate(enemyAura.transform.root.gameObject);
	}
}
