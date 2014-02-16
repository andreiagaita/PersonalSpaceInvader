using UnityEngine;
using System.Collections;

public class DelayNewRound : MonoBehaviour {

	public float delay = 1f;
	public GameObject newRoundMenu;
	private float elapsedTime = 0;

	void Awake ()
	{
		newRoundMenu.GetComponent<MenuHandler>().enabled = false;
	}

	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > delay)
		{
			newRoundMenu.GetComponent<MenuHandler>().enabled = true;
			Destroy (gameObject);
		}
	}
}
