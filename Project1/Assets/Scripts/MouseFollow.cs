﻿using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {

	public Transform target;
	public float rotationSpeed = 0.1f;

	void Start ()
	{
		if (!target)
			Debug.LogError("You need to assign a target to the AuraSpike object");
	}

	void Update () 
	{
		RotateTowardsTarget(target);
	}

	private void RotateTowardsTarget(Transform target)
	{
		var direction = (target.position - transform.position).normalized;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
		angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * rotationSpeed);
		var lookRotation = Quaternion.Euler(0, 0, angle);
		
		transform.rotation = lookRotation;
	}
}