using UnityEngine;
using System.Collections;
using System;

public class PlayerBehaviour : MonoBehaviour {

	public event Action Died;
	public event Action<PlayerBehaviour, string> Jumped;
	public PlayerBehaviour enemy;
	public GameObject aura;
	public GameObject arrow;
	public SpriteRenderer graphic;
	public Transform spawnLocation;
	public PlayerColor playerColor;
	private float playerAuraDistance = 0f;
	private float distanceLimit; 

	void Awake () {
		if (GameManager.instance)
			GameManager.instance.AddPlayer (this);
	}
	
	void Start () {
		distanceLimit = (aura.renderer.bounds.size.x) / 2;
		aura.GetComponent<SpriteRenderer>().color = GetActualPlayerColor();
		graphic.color = GetActualPlayerColor ();
	}
	
	void Update () {
		if (!enemy) return;

		playerAuraDistance = Vector3.Distance (enemy.transform.position, transform.position);
		if (playerAuraDistance < distanceLimit) {
			enemy.Die ();
			if (GameManager.instance)
				GameManager.instance.AwardPointToPlayer (this);
		}
	}
	
	public void SetTarget (PlayerBehaviour target) {
		enemy = target;
		arrow.GetComponent<SpriteRenderer>().color = enemy.GetActualPlayerColor();
	}
	
	public void Die () {
		if (Died != null)
			Died ();
		Respawn();
	}

	public PlayerColor GetPlayerColor()
	{
		return playerColor;
	}

	public Color GetActualPlayerColor()
	{
		if (GameManager.instance)
			return GameManager.instance.playerColors[(int)playerColor];
		return Color.green;
	}

	public void RaiseJumped (string type)
	{
		if (Jumped != null)
			Jumped (this, type);
	}

	void Respawn()
	{
		if (GameManager.instance)
			transform.position = GameManager.instance.GetSpawnPositionFurtestAway ();
	}
}
