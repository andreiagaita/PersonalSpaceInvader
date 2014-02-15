using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public void DoMenu (string menu)
	{
		switch (menu) {
		case "start":
			Application.LoadLevel ("LevelTest");
			break;
		default:
			break;
		}
	}
}
