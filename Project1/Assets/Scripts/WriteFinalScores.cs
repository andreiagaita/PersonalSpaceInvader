using UnityEngine;
using System.Collections;

public class WriteFinalScores : MonoBehaviour {
	public GameObject redText;
	public GameObject greenText;
	public GameObject blueText;
	public GameObject orangeText;

	void Start () {
		redText.GetComponent<TextMesh>().text = "RED            " + GameManager.instance.scoreDict[PlayerColor.Red];
		greenText.GetComponent<TextMesh>().text = "GREEN       " + GameManager.instance.scoreDict[PlayerColor.Green];
		blueText.GetComponent<TextMesh>().text = "BLUE          " + GameManager.instance.scoreDict[PlayerColor.Blue];
		orangeText.GetComponent<TextMesh>().text = "ORANGE    " + GameManager.instance.scoreDict[PlayerColor.Orange];
	}
}
