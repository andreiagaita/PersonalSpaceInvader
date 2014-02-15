using UnityEngine;
using System.Collections;

public class UpdateWinnerText : MonoBehaviour 
{
	void Start () {
		GetComponent<TextMesh>().text = GameManager.instance.winningPlayerColor.ToString().ToLower() + " player won";
	}
}
