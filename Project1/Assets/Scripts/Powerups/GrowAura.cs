using UnityEngine;
using System.Collections;

public class GrowAura : PowerUpBase
{
	public float auraScale = 2f;
	private Vector3 originalScale;
	private GameObject aura;

	public override void StartEffect (GameObject player)
	{
		aura = player.GetComponent<PlayerBehaviour>().aura;
		originalScale = aura.transform.localScale;
		aura.transform.localScale *= auraScale;
	}

	public override void StopEffect (GameObject player)
	{
		aura.transform.localScale = originalScale;
	}
}
