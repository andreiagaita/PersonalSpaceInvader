using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ViewerType
{
	None,
	Game,
	Player,
	Spectator
}



public class PlayerInfo
{
	public enum PlayerState
	{
		Connected,
		PlayerReady,
		WorldReady,
		Playing
	}

	private bool m_IsMaster = false;

	public int ID { get; set; }
	public string Name { get; set; }
	public Vector3 Position { get; set; }
	public Color Color { get; set; }
	public int Score { get; set; }
	public PlayerState currentState { get; set; }
	public bool isMaster {
		get { return m_IsMaster; }
	}
	public int AnimationIndex { get; set; }

	public GameObject gameObject { get; set; }

	public List<PickupObject> ObjectsOwned { get; set; }
	public List<PickupObject> ObjectsPicked { get; set; }

	public PlayerInfo(int id, string name, Vector3 position, Color color, int score, int animationIndex, bool master)
	{
		ID = id;
		Name = name;
		Position = position;
		Color = color;
		Score = score;
		m_IsMaster = master;
		AnimationIndex = animationIndex;
		currentState = PlayerState.Connected;

		ObjectsOwned = new List<PickupObject>();
		ObjectsPicked = new List<PickupObject>();
	}
}
