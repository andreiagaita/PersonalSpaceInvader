using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour 
{
	void Awake()
	{
		GameManager.instance.AddPowerUpLocation(gameObject.transform);
	}
}
