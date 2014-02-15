using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public void DoMenu (string menu)
	{
		switch (menu) {
		case "start":
		case "yes":
			Application.LoadLevel ("LevelTest");
			break;
		case "no":
			Application.LoadLevel ("MainMenu");
			break;
		case "credits":
			Debug.Log ("hit credits");
			break;
		case "quit":
			Application.Quit(); 
			break;
		default:
			break;
		}
	}
}
