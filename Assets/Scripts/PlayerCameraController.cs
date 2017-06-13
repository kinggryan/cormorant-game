using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

	public float maxSpeedDistanceFromPlayer;
	public float minSpeedDistanceFromPlayer;
	public float maxTurnSpeedTilt;
	public float lerpRate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateMovementParameters(float movementSpeedRatio, float turnRatio) {
		var targetDistance = minSpeedDistanceFromPlayer + (maxSpeedDistanceFromPlayer - minSpeedDistanceFromPlayer) * movementSpeedRatio;
		var targetTilt = maxTurnSpeedTilt * turnRatio;
		transform.position = Vector3.Lerp (transform.position, transform.parent.position + transform.parent.forward * -targetDistance,lerpRate*Time.deltaTime);
		var targetTiltDiff = targetTilt - Vector3.Angle (Vector3.up, transform.up);
		transform.Rotate (new Vector3 (0, 0, targetTiltDiff), Space.Self);
	}
}
