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
	[HideInInspector]
	public PowerUpBase activePowerUp;

	private float playerAuraDistance = 0f;
	private float distanceLimit;
	
	private bool dying = false;
	private bool dead = true;
	private float dieTime = 0;
	
	private static float sloMoTimeStop = -1;

	void Awake () {
		if (GameManager.instance)
			GameManager.instance.AddPlayer (this);
	}
	
	void Start () {
		distanceLimit = (aura.renderer.bounds.size.x) / 2;
		aura.GetComponent<SpriteRenderer>().color = GetActualPlayerColor();
		graphic.color = GetActualPlayerColor ();
		
		dieTime = Time.time;
		playable = false;
		transform.localScale = Vector3.one * 0.0001f;
	}
	
	public bool playable {
		get { return collider.enabled; }
		set {
			GetComponent<CharacterController> ().enabled = value;
			collider2D.enabled = value;
			rigidbody2D.isKinematic = !value;
		}
	} 
	
	static float spawnDelay = 0.5f;
	static float spawnDuration = 0.5f;
	
	void Update () {
		if (Time.time > sloMoTimeStop)
			Time.timeScale = 1;
		
		if (dying) {
			transform.localScale = Vector3.one * (1 + (Time.time - dieTime) * 4);
			if (Time.time > dieTime + 0.1f) {
				dying = false;
				dead = true;
				transform.localScale = Vector3.one * 0.0001f;
				Respawn();
			}
		}
		if (dead) {
			float spawnTime = Mathf.Clamp01 ((Time.time - dieTime - spawnDelay) / spawnDuration);
			transform.localScale = Vector3.one * spawnTime;
			if (spawnTime == 1) {
				playable = true;
				dead = false;
			}
		}
		
		if (!enemy) return;

		playerAuraDistance = Vector3.Distance (enemy.transform.position, transform.position);
		if (playerAuraDistance < distanceLimit && !enemy.dead && !enemy.dying) {
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
		if (dead || dying)
			return;
		if (Died != null)
			Died ();
		
		dying = true;
		dieTime = Time.time;
		playable = false;
		sloMoTimeStop = Time.time + 0.2f;
		//Time.timeScale = 0.3f;
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
