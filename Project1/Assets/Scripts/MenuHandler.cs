using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public MainMenu mainMenu;

	void OnMouseDown() {
		mainMenu.DoMenu (gameObject.tag);
	}
}
