using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {

	public MainMenu mainMenu;

	public MenuHandler next;
	public MenuHandler previous;
	public bool selected;

	private TextMesh text;

	void Awake ()
	{
		text = GetComponent<TextMesh> ();
	}

	void OnMouseDown()
	{
		mainMenu.DoMenu (gameObject.tag);
	}

	void Update ()
	{
		if (selected && Input.GetButtonDown ("Menu Select"))
			mainMenu.DoMenu (gameObject.tag);

		if (selected && Mathf.Abs (Input.GetAxis ("Menu Move")) > 0.1f)
		{
			selected = false;

			if (Input.GetAxis ("Menu Move") < 0)
				previous.Select ();
			else
				next.Select ();
		}

		if (selected)
			text.color = Color.green;
		else
			text.color = Color.white;
	}

	public void Select ()
	{
		StartCoroutine ("DelayedSelect");
	}

	private IEnumerator DelayedSelect ()
	{
		yield return new WaitForSeconds (0.2f);
		selected = true;
	}
}
