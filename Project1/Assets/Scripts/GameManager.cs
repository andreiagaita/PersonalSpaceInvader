using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	
	public static GameManager instance = null;
	
	public void Awake () {
		instance = this;
	}
	
	public static int playersAlive = 3;
	public static int scorePlayer1 = 0;
	public static int scorePlayer2 = 0;
	public static int scorePlayer3 = 0;
}
