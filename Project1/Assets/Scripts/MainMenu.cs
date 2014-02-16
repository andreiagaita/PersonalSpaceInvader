using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public void DoMenu (string menu)
	{
		switch (menu) {
		case "start":
		case "yes":
			GameManager.instance.NextLevel ();
			break;
		case "no":
		case "back":
			Application.LoadLevel ("MainMenu");
			break;
		case "credits":
			Application.LoadLevel ("Credits");
			break;
		case "quit":
			Application.Quit(); 
			break;
		default:
			break;
		}
	}
}
