using UnityEngine;
using System.Collections;

public class GrowAura : MonoBehaviour 
{
	public float auraScale = 2f;
	public float durationOfPowerUp = 10.0f;
	private Vector3 originalScale;
	private float elapsedTime = 0f;
	private bool powerUpPicked = false;
	private GameObject aura;

	void Update()
	{
		if (powerUpPicked)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > durationOfPowerUp)
			{
				aura.transform.localScale = originalScale;
				DestroyImmediate(gameObject);
			}
		}
	}

	void OnCollisionEnter2D (Collision2D col) 
	{
		aura = col.gameObject.GetComponent<PlayerBehaviour>().aura;
		originalScale = aura.transform.localScale;
		aura.transform.localScale *= auraScale;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		powerUpPicked = true;
	}
}
